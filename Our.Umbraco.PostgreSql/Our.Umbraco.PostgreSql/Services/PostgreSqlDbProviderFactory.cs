using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Npgsql;

namespace Our.Umbraco.PostgreSql.Services
{
    public class PostgreSqlDbProviderFactory : DbProviderFactory
    {
        public static readonly PostgreSqlDbProviderFactory Instance = new();

        private NpgsqlFactory Base => NpgsqlFactory.Instance;

        public override DbCommand CreateCommand()
        {
            var rVal = Base.CreateCommand();

            return rVal;
        }
        public override DbConnection CreateConnection() => Base.CreateConnection();

        public override DbParameter CreateParameter() => Base.CreateParameter();

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => Base.CreateConnectionStringBuilder();

        public override DbCommandBuilder CreateCommandBuilder()
        {
            var rVal = Base.CreateCommandBuilder();

            return rVal;
        }

        public override DbDataAdapter CreateDataAdapter() => Base.CreateDataAdapter();

        /// <summary>
        /// Specifies whether the specific <see cref="DbProviderFactory"/> supports the <see cref="DbDataAdapter"/> class.
        /// </summary>
        public override bool CanCreateDataAdapter => true;

        /// <summary>
        /// Specifies whether the specific <see cref="DbProviderFactory"/> supports the <see cref="DbCommandBuilder"/> class.
        /// </summary>
        public override bool CanCreateCommandBuilder => true;

        /// <inheritdoc/>
        public override bool CanCreateBatch => true;

        /// <inheritdoc/>
        public override DbBatch CreateBatch() => Base.CreateBatch();

        /// <inheritdoc/>
        public override DbBatchCommand CreateBatchCommand() => Base.CreateBatchCommand();

        /// <inheritdoc/>
        public override DbDataSource CreateDataSource(string connectionString)
            => Base.CreateDataSource(connectionString);
    }
}
