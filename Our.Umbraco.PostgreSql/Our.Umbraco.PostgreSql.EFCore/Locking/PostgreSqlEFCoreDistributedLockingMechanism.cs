using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Core.DistributedLocking;
using Umbraco.Cms.Core.DistributedLocking.Exceptions;
using Umbraco.Cms.Core.Exceptions;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Cms.Persistence.EFCore.Scoping;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql.EFCore.Locking;

/// <summary>
/// Provides a distributed locking mechanism for PostgreSQL databases using Entity Framework Core, enabling safe
/// coordination of concurrent operations across multiple processes or instances.
/// </summary>
/// <remarks>This implementation integrates with the Umbraco scope and transaction system to ensure that
/// distributed locks are acquired and released in the context of an active EF Core transaction. Locks are managed at
/// the database level, allowing multiple application instances to coordinate access to shared resources. The mechanism
/// requires that the connection string is configured for PostgreSQL and that an ambient EF Core scope is present.
/// Thread safety and correct transaction isolation are enforced by the underlying infrastructure.</remarks>
/// <typeparam name="T">The type of the Entity Framework Core DbContext to use for database access.</typeparam>
public sealed class PostgreSqlEFCoreDistributedLockingMechanism<T> : IDistributedLockingMechanism
    where T : DbContext
{
    private ConnectionStrings _connectionStrings;
    private GlobalSettings _globalSettings;
    private readonly ILogger<PostgreSqlEFCoreDistributedLockingMechanism<T>> _logger;
    private readonly IScopeAccessor _scopeAccessor;
    private readonly Lazy<IEFCoreScopeAccessor<T>> _scopeAccessorEFCore; // Hooray it's a circular dependency.

    /// <summary>
    ///     Initializes a new instance of the <see cref="PostgreSqlEFCoreDistributedLockingMechanism{T}"/> class.
    /// </summary>
    public PostgreSqlEFCoreDistributedLockingMechanism(
        ILogger<PostgreSqlEFCoreDistributedLockingMechanism<T>> logger,
        IScopeAccessor scopeAccessor,
        Lazy<IEFCoreScopeAccessor<T>> scopeAccessorEFCore,
        IOptionsMonitor<GlobalSettings> globalSettings,
        IOptionsMonitor<ConnectionStrings> connectionStrings)
    {
        _logger = logger;
        _scopeAccessor = scopeAccessor;
        _scopeAccessorEFCore = scopeAccessorEFCore;
        _globalSettings = globalSettings.CurrentValue;
        _connectionStrings = connectionStrings.CurrentValue;
        //_connectionStrings.ProviderName = Constants.ProviderName; // force provider name to our provider
        globalSettings.OnChange(x => _globalSettings = x);
        connectionStrings.OnChange(x => _connectionStrings = x);
    }

    public bool HasActiveRelatedScope => _scopeAccessorEFCore.Value.AmbientScope is not null;

    /// <inheritdoc />
    public bool Enabled => _connectionStrings.IsConnectionStringConfigured() &&
                           string.Equals(_connectionStrings.ProviderName, Constants.ProviderName, StringComparison.InvariantCultureIgnoreCase) && _scopeAccessorEFCore.Value.AmbientScope is not null;

    /// <inheritdoc />
    public IDistributedLock ReadLock(int lockId, TimeSpan? obtainLockTimeout = null)
    {
        obtainLockTimeout ??= _globalSettings.DistributedLockingReadLockDefaultTimeout;
        return new PostgreSqlDistributedLock(this, _scopeAccessor, lockId, DistributedLockType.ReadLock, obtainLockTimeout.Value);
    }

    /// <inheritdoc />
    public IDistributedLock WriteLock(int lockId, TimeSpan? obtainLockTimeout = null)
    {
        obtainLockTimeout ??= _globalSettings.DistributedLockingWriteLockDefaultTimeout;
        return new PostgreSqlDistributedLock(this, _scopeAccessor, lockId, DistributedLockType.WriteLock, obtainLockTimeout.Value);
    }


    private sealed class PostgreSqlDistributedLock : IDistributedLock
    {
        private readonly global::Umbraco.Cms.Infrastructure.Persistence.SqlSyntax.ISqlSyntaxProvider _syntax;
        private readonly PostgreSqlEFCoreDistributedLockingMechanism<T> _parent;
        private readonly TimeSpan _timeout;

        public PostgreSqlDistributedLock(
            PostgreSqlEFCoreDistributedLockingMechanism<T> parent,
            IScopeAccessor scopeAccessor,
            int lockId,
            DistributedLockType lockType,
            TimeSpan timeout)
        {
            _syntax = scopeAccessor.AmbientScope?.SqlContext.SqlSyntax ?? throw new InvalidOperationException("No SQL syntax available.");
            _parent = parent;
            _timeout = timeout;
            LockId = lockId;
            LockType = lockType;

            _parent._logger.LogDebug("Requesting {lockType} for id {id}", LockType, LockId);

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
            catch (PostgresException ex) when (ex.SqlState == "55P03" || ex.SqlState == "40P01")
            {
                if (LockType == DistributedLockType.ReadLock)
                {
                    throw new DistributedReadLockTimeoutException(LockId);
                }

                throw new DistributedWriteLockTimeoutException(LockId);
            }

            _parent._logger.LogDebug("Acquired {lockType} for id {id}", LockType, LockId);
        }

        public int LockId { get; }

        public DistributedLockType LockType { get; }

        public void Dispose() =>
            // Mostly no op, cleaned up by completing transaction in scope.
            _parent._logger.LogDebug("Dropped {lockType} for id {id}", LockType, LockId);

        public override string ToString()
            => $"PostgreSQLDistributedLock({LockId}, {LockType}";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "EF1002:Risk of vulnerability to SQL injection.", Justification = "<Pending>")]
        private void ObtainReadLock()
        {
            IEfCoreScope<T>? scope = _parent._scopeAccessorEFCore.Value.AmbientScope
                ?? throw new PanicException("No ambient scope");

            scope.ExecuteWithContextAsync<Task>(async dbContext =>
            {
                if (dbContext.Database.CurrentTransaction is null)
                {
                    throw new InvalidOperationException(
                        "PostgreSQLDistributedLockingMechanism requires a transaction to function.");
                }

                if (dbContext.Database.CurrentTransaction.GetDbTransaction().IsolationLevel <
                    IsolationLevel.ReadCommitted)
                {
                    throw new InvalidOperationException(
                        "A transaction with minimum ReadCommitted isolation level is required.");
                }

                // SET LOCAL kann nur in Transaktionsblöcken verwendet werden
                await dbContext.Database.ExecuteSqlRawAsync($"SET LOCAL lock_timeout = '{(int)_timeout.TotalMilliseconds}ms'");

                // GitHub Copilot: The WITH (REPEATABLEREAD) table hint is specific to SQL Server and is not valid in PostgreSQL. In PostgreSQL, transaction isolation is controlled at the transaction level, not per-query, and you cannot use SQL Server-style table hints.
                // 1. Removed WITH (REPEATABLEREAD) because PostgreSQL does not support this syntax.
                // ToDo: check 2. PostgreSQL handles repeatable read at the transaction level, so ensure your transaction is started with the correct isolation level elsewhere in your code.
                var selectCmd = $"SELECT value FROM {_syntax.GetQuotedTableName("umbracoLock")} WHERE id={LockId} FOR SHARE";
                var number = await dbContext.Database.ExecuteScalarAsync<int?>(selectCmd);

                if (number == null)
                {
                    // ensure we are actually locking!
                    throw new ArgumentException(@$"LockObject with id={LockId} does not exist.", nameof(LockId));
                }
            }).GetAwaiter().GetResult();
        }

        private void ObtainWriteLock()
        {
            IEfCoreScope<T>? scope = _parent._scopeAccessorEFCore.Value.AmbientScope
                ?? throw new PanicException("No ambient scope");

            scope.ExecuteWithContextAsync<Task>(async dbContext =>
            {
                if (dbContext.Database.CurrentTransaction is null)
                {
                    throw new InvalidOperationException(
                        "PostgreSQLDistributedLockingMechanism requires a transaction to function.");
                }

                if (dbContext.Database.CurrentTransaction.GetDbTransaction().IsolationLevel < IsolationLevel.ReadCommitted)
                {
                    throw new InvalidOperationException(
                        "A transaction with minimum ReadCommitted isolation level is required.");
                }

                // SET LOCAL kann nur in Transaktionsblöcken verwendet werden
                await dbContext.Database.ExecuteSqlRawAsync($"SET LOCAL lock_timeout = '{(int)_timeout.TotalMilliseconds}ms'");

                var updateCmd = $"UPDATE {_syntax.GetQuotedTableName("umbracoLock")} SET value = (CASE WHEN (value=1) THEN -1 ELSE 1 END) WHERE id={LockId}";
                var rowsAffected = await dbContext.Database.ExecuteSqlRawAsync(updateCmd);

                if (rowsAffected == 0)
                {
                    // ensure we are actually locking!
                    throw new ArgumentException($"LockObject with id={LockId} does not exist.");
                }
            }).GetAwaiter().GetResult();
        }
    }
}
