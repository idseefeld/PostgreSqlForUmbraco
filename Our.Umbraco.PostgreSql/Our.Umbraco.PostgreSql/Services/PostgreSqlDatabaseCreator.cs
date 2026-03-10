using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Our.Umbraco.PostgreSql.Services
{
    /// <summary>
    /// Implements <see cref="IDatabaseCreator" /> for PostgreSQL.
    /// </summary>
    public class PostgreSqlDatabaseCreator : IDatabaseCreator
    {
        private readonly ILogger<PostgreSqlDatabaseCreator> _logger;
        public string ProviderName => Constants.ProviderName;

        public PostgreSqlDatabaseCreator(ILogger<PostgreSqlDatabaseCreator> logger) => _logger = logger;

        public void Create(string connectionString)
        {
            try
            {
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);

                using (NpgsqlDataSource dataSource = dataSourceBuilder.Build())
                {
                    // Open a connection to the database to ensure the connection string is valid
                    using (NpgsqlConnection connection = dataSource.OpenConnection())
                    {
                        var databaseName = connection.Database;
                        _logger.LogDebug("PostgreSql connection established successfully.");
                        using (NpgsqlCommand command = connection.CreateCommand())
                        {
                            command.CommandText = $"CREATE DATABASE \"{databaseName}\"";
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (PostgresException ex) when (ex.SqlState == "42P04") // duplicate_database
            {
                _logger.LogWarning("Database already exists.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the database.");
                throw;
            }
        }
    }
}
