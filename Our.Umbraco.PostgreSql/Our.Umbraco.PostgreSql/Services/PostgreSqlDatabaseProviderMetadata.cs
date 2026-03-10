using System;
using System.Runtime.Serialization;
using Npgsql;
using Our.Umbraco.PostgreSql;
using Umbraco.Cms.Core.Install.Models;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Our.Umbraco.PostgreSql.Services
{
    [DataContract]
    public class PostgreSqlDatabaseProviderMetadata : IDatabaseProviderMetadata
    {

        /// <inheritdoc />
        public Guid Id => new Guid("d1f8c5b2-3e4f-4b6a-9c7e-8f0c1d2e3f4a");


        /// <inheritdoc />
        public int SortOrder => 5;

        /// <inheritdoc />
        public string DisplayName => "PostgreSQL";

        /// <inheritdoc />
        public string DefaultDatabaseName => Constants.UmbracoDefaultDatabaseName;


        /// <inheritdoc />
        public string? ProviderName => Constants.ProviderName;


        /// <inheritdoc />
        public bool SupportsQuickInstall => false;


        /// <inheritdoc />
        public bool IsAvailable => true;


        /// <inheritdoc />
        public bool RequiresServer => true;


        /// <inheritdoc />
        public string? ServerPlaceholder => "localhost:5433";


        /// <inheritdoc />
        public bool RequiresCredentials => true;

        /// <inheritdoc/>
        public bool SupportsIntegratedAuthentication => true;

        /// <inheritdoc />
        public bool SupportsTrustServerCertificate => true;

        /// <inheritdoc/>
        public bool RequiresConnectionTest => true;

        /// <inheritdoc />
        public bool ForceCreateDatabase => true;

        /// <inheritdoc />
        public string? GenerateConnectionString(DatabaseModel databaseModel)
        {
            var server = !string.IsNullOrEmpty(databaseModel.Server) ? databaseModel.Server : ServerPlaceholder ?? "localhost:5432";
            var serverParts = server.Split([':'], 2);
            var hostName = serverParts[0];
            var port = serverParts.Length > 1 ? serverParts[1] : "5433";

            var csb = new NpgsqlConnectionStringBuilder
            {
                Host = hostName ?? "localhost",
                Port = int.Parse(port),
                SslMode = databaseModel.TrustServerCertificate ? SslMode.Allow : SslMode.VerifyCA,
                Database = databaseModel.DatabaseName ?? Constants.UmbracoDefaultDatabaseName,
            };

            if (databaseModel.IntegratedAuth)
            {
                // PostgreSQL does not support integrated authentication in the same way as SQL Server or Windows Authentication. Not the client decides how to authenticate, bute the server via pg_hba.conf see. https://github.com/npgsql/npgsql/issues/4789
            }
            else
            {
                csb.Username = databaseModel.Login ?? string.Empty;
                csb.Password = databaseModel.Password ?? string.Empty;
            }

            return csb.ConnectionString;
        }
    }
}
