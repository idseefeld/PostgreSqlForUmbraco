using System;
using System.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DistributedLocking;
using Umbraco.Cms.Core.DistributedLocking.Exceptions;
using Umbraco.Cms.Core.Exceptions;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql.Locking
{
    /// <inheritdoc />
    public class PostgreSqlDistributedLockingMechanism : IDistributedLockingMechanism
    {

        private ConnectionStrings _connectionStrings;
        private GlobalSettings _globalSettings;
        private readonly ILogger<PostgreSqlDistributedLockingMechanism> _logger;
        private readonly Lazy<IScopeAccessor> _scopeAccessor; // Hooray it's a circular dependency.
        /// <inheritdoc />
        public PostgreSqlDistributedLockingMechanism(
            ILogger<PostgreSqlDistributedLockingMechanism> logger,
            Lazy<IScopeAccessor> scopeAccessor,
            IOptionsMonitor<GlobalSettings> globalSettings,
            IOptionsMonitor<ConnectionStrings> connectionStrings)
        {
            _logger = logger;
            _scopeAccessor = scopeAccessor;
            _globalSettings = globalSettings.CurrentValue;
            _connectionStrings = connectionStrings.CurrentValue;
            globalSettings.OnChange(x => _globalSettings = x);
            connectionStrings.OnChange(x => _connectionStrings = x);

        }


        /// <inheritdoc />
        public bool Enabled
        {
            get
            {
                var isConnectionStringConfigured = _connectionStrings.IsConnectionStringConfigured();
                var connectionStringsProviderName = _connectionStrings.ProviderName;
                var equalProviderName = string.Equals(connectionStringsProviderName, Constants.ProviderName, StringComparison.InvariantCultureIgnoreCase);

                return isConnectionStringConfigured && equalProviderName;
            }
        }

        /// <inheritdoc />
        public IDistributedLock ReadLock(int lockId, TimeSpan? obtainLockTimeout = null)
        {
            if (!Enabled)
            {
                throw new InvalidOperationException("Distributed locking is not enabled. Please check your connection strings and provider name.");
            }

            obtainLockTimeout ??= _globalSettings.DistributedLockingReadLockDefaultTimeout;
            return new PostgreSqlDistributedLock(this, _scopeAccessor, lockId, DistributedLockType.ReadLock, obtainLockTimeout.Value);
        }

        /// <inheritdoc />
        public IDistributedLock WriteLock(int lockId, TimeSpan? obtainLockTimeout = null)
        {
            obtainLockTimeout ??= _globalSettings.DistributedLockingWriteLockDefaultTimeout;
            return new PostgreSqlDistributedLock(this, _scopeAccessor, lockId, DistributedLockType.WriteLock, obtainLockTimeout.Value);
        }

        internal class PostgreSqlDistributedLock : IDistributedLock
        {
            private readonly global::Umbraco.Cms.Infrastructure.Persistence.SqlSyntax.ISqlSyntaxProvider _syntax;
            private readonly PostgreSqlDistributedLockingMechanism _parent;
            private readonly TimeSpan _timeout;
            public PostgreSqlDistributedLock(
                PostgreSqlDistributedLockingMechanism parent,
                Lazy<IScopeAccessor> scopeAccessor,
                int lockId,
                DistributedLockType lockType,
                TimeSpan timeout)
            {
                _syntax = scopeAccessor.Value.AmbientScope?.SqlContext.SqlSyntax ?? throw new InvalidOperationException("No SQL syntax available.");
                _parent = parent;
                _timeout = timeout;
                LockId = lockId;
                LockType = lockType;
                if (_parent._logger.IsEnabled(LogLevel.Debug))
                {
                    _parent._logger.LogDebug("Requesting {lockType} for id {id}", LockType, LockId);
                }

                try
                {
                    switch (lockType)
                    {
                        case DistributedLockType.ReadLock:
                            ObtainReadLock();
                            break;
                        case DistributedLockType.WriteLock:
                            ObtainWriteLock();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(lockType), lockType, @"Unsupported lockType");
                    }
                }
                catch (Exception ex) when (IsLockTimeout(ex))
                {
                    if (LockType == DistributedLockType.ReadLock)
                    {
                        throw new DistributedReadLockTimeoutException(LockId);
                    }

                    throw new DistributedWriteLockTimeoutException(LockId);
                }

                if (_parent._logger.IsEnabled(LogLevel.Debug))
                {
                    _parent._logger.LogDebug("Acquired {lockType} for id {id}", LockType, LockId);
                }
            }

            public int LockId { get; }

            public DistributedLockType LockType { get; }

            public void Dispose()
            {
                if (_parent._logger.IsEnabled(LogLevel.Debug))
                {
                    // Mostly no op, cleaned up by completing transaction in scope.
                    _parent._logger.LogDebug("Dropped {lockType} for id {id}", LockType, LockId);
                }
            }

            private void ObtainReadLock()
            {
                IUmbracoDatabase? db = _parent._scopeAccessor.Value.AmbientScope?.Database;

                if (db is null)
                {
                    throw new PanicException("Could not find a database");
                }

                if (db.InTransaction is false || db.Transaction is null)
                {
                    throw new InvalidOperationException(
                        "PostgreSqlDistributedLockingMechanism requires a transaction to function.");
                }

                if (db.Transaction.IsolationLevel < IsolationLevel.ReadCommitted)
                {
                    throw new InvalidOperationException(
                        "A transaction with minimum ReadCommitted isolation level is required.");
                }

                BoundNextCommandByLockTimeout(db);

                string query = $"SELECT value FROM {_syntax.GetQuotedTableName("umbracoLock")} WHERE id = @id FOR SHARE";

                // execute the lock timeout query and the actual query in a single server roundtrip
                _ = db.ExecuteScalar<int?>($"{GetLockTimeoutQuery()};{query}", new { id = LockId })
                            ?? throw new ArgumentException(@$"LockObject with id={LockId} does not exist.", nameof(LockId));
            }

            private string GetLockTimeoutQuery()
            {
                return $"SET LOCAL lock_timeout = '{(int)_timeout.TotalMilliseconds}ms'";
            }

            private void ObtainWriteLock()
            {
                IUmbracoDatabase? db = _parent._scopeAccessor.Value.AmbientScope?.Database
                    ?? throw new PanicException("Could not find a database");

                if (db.InTransaction is false || db.Transaction is null)
                {
                    throw new InvalidOperationException(
                        "PostgreSqlDistributedLockingMechanism requires a transaction to function.");
                }

                if (db.Transaction.IsolationLevel < IsolationLevel.ReadCommitted)
                {
                    throw new InvalidOperationException(
                        "A transaction with minimum ReadCommitted isolation level is required.");
                }

                string updateCmd = $"UPDATE {_syntax.GetQuotedTableName("umbracoLock")} SET value = (CASE WHEN (value=1) THEN -1 ELSE 1 END) WHERE id=@id";

                BoundNextCommandByLockTimeout(db);

                // execute the lock timeout query and the actual query in a single server roundtrip

                var rowsAffected = db.Execute($"{GetLockTimeoutQuery()};{updateCmd}", new { id = LockId });
                if (rowsAffected == 0)
                {
                    // ensure we are actually locking!
                    throw new ArgumentException($"LockObject with id={LockId} does not exist.");
                }
            }

            private static bool IsLockTimeout(Exception exception)
            {
                if (exception is Npgsql.PostgresException { SqlState: "55P03" or "40P01" })
                {
                    return true;
                }

                if (exception is TimeoutException)
                {
                    return true;
                }

                return exception.InnerException is not null && IsLockTimeout(exception.InnerException);
            }

            /// <summary>
            ///     Bounds the next statement executed on the database by a command timeout derived from this
            ///     lock's own timeout.
            /// </summary>
            /// <param name="db">The database that will execute the lock statement.</param>
            /// <remarks>
            ///     Without this the ambient command timeout can abort the statement while the server is still
            ///     waiting for the row lock, surfacing a raw timeout instead of a lock timeout exception.
            ///     <c>OneTimeCommandTimeout</c> is reset once the command executes, so it cannot leak into the
            ///     rest of the scope.
            /// </remarks>
            private void BoundNextCommandByLockTimeout(IUmbracoDatabase db)
                => db.OneTimeCommandTimeout = _timeout.ToLockCommandTimeoutSeconds();
        }
    }
}
