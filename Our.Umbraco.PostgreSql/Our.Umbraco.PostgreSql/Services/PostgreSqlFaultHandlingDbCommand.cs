using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Our.Umbraco.PostgreSql.Extensions;
using Umbraco.Cms.Infrastructure.Persistence.FaultHandling;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Our.Umbraco.PostgreSql.Services
{
    internal sealed class PostgreSqlFaultHandlingDbCommand : DbCommand
    {
        private readonly RetryPolicy _cmdRetryPolicy;
        private readonly IPackagesService _packagesService;
        private RetryDbConnection _connection;

        public PostgreSqlFaultHandlingDbCommand(RetryDbConnection connection, DbCommand command, RetryPolicy? cmdRetryPolicy, IPackagesService packagesService)
        {
            _connection = connection;
            Inner = command.FixCommanText(packagesService);
            _cmdRetryPolicy = cmdRetryPolicy ?? RetryPolicy.NoRetry;
            _packagesService = packagesService;
        }

        public DbCommand Inner { get; private set; }

        [AllowNull]
        public override string CommandText
        {
            get => Inner.CommandText;
            set => Inner.CommandText = value;
        }

        public override int CommandTimeout
        {
            get => Inner.CommandTimeout;
            set => Inner.CommandTimeout = value;
        }

        public override CommandType CommandType
        {
            get => Inner.CommandType;
            set => Inner.CommandType = value;
        }

        public override bool DesignTimeVisible { get; set; }

        [AllowNull]
        protected override DbConnection DbConnection
        {
            get => _connection;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!(value is RetryDbConnection connection))
                {
                    throw new ArgumentException("Value is not a FaultHandlingDbConnection instance.");
                }

                if (_connection != null && _connection != connection)
                {
                    throw new Exception("Value is another FaultHandlingDbConnection instance.");
                }

                _connection = connection;
                Inner.Connection = connection.Inner;
            }
        }

        protected override DbParameterCollection DbParameterCollection => Inner.Parameters;

        protected override DbTransaction? DbTransaction
        {
            get => Inner.Transaction;
            set => Inner.Transaction = value;
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get => Inner.UpdatedRowSource;
            set => Inner.UpdatedRowSource = value;
        }

        public override void Cancel() => Inner.Cancel();

        public override int ExecuteNonQuery() => Execute(() => Inner.ExecuteNonQuery());

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Inner.Dispose();
            }

            Inner = null!;
            base.Dispose(disposing);
        }

        protected override DbParameter CreateDbParameter() => Inner.CreateParameter();

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) =>
            Execute(() => Inner.ExecuteReader(behavior));

        public override object? ExecuteScalar() => Execute(() => Inner.ExecuteScalar());

        public override void Prepare() => Inner.Prepare();

        private T Execute<T>(Func<T> f) =>
            _cmdRetryPolicy.ExecuteAction(() =>
            {
                _connection.Ensure();
                return f();
            })!;
    }
}
