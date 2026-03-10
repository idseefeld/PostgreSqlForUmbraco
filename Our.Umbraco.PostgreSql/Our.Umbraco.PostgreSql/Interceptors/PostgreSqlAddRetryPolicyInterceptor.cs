using System.Data.Common;
using Microsoft.Extensions.Options;
using NPoco;
using Our.Umbraco.PostgreSql.FaultHandling;
using Our.Umbraco.PostgreSql.Services;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Cms.Infrastructure.Persistence.FaultHandling;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql.Interceptors
{
    public class PostgreSqlAddRetryPolicyInterceptor : PostgreSqlConnectionInterceptor
    {
        private readonly IOptionsMonitor<ConnectionStrings> _connectionStrings;
        private readonly IPackagesService _packagesFixService;

        public PostgreSqlAddRetryPolicyInterceptor(IOptionsMonitor<ConnectionStrings> connectionStrings, IPackagesService packagesFixService)
        {
            _connectionStrings = connectionStrings;
            _packagesFixService = packagesFixService;
        }

        public override DbConnection OnConnectionOpened(IDatabase database, DbConnection conn)
        {
            if (!_connectionStrings.CurrentValue.IsConnectionStringConfigured())
            {
                return conn;
            }

            RetryPolicy? connectionRetryPolicy = RetryPolicyFactory.GetPostgreSqlConnectionRetryPolicy();
            RetryPolicy? commandRetryPolicy = RetryPolicyFactory.GetPostgreSqlCommandRetryPolicy();

            if (connectionRetryPolicy == null && commandRetryPolicy == null)
            {
                return conn;
            }

            return new PostgreSqlRetryDbConnection(conn, connectionRetryPolicy, commandRetryPolicy, _packagesFixService);
        }
    }
}
