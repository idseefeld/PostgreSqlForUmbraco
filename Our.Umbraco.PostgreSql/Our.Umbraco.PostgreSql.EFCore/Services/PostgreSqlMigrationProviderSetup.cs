using Microsoft.EntityFrameworkCore;
using Our.Umbraco.PostgreSql.EFCore.Extensions;
using Umbraco.Cms.Persistence.EFCore.Migrations;

namespace Our.Umbraco.PostgreSql.EFCore.Services
{
    public class PostgreSqlMigrationProviderSetup : IMigrationProviderSetup
    {
        public string ProviderName => Constants.ProviderName;

        public void Setup(DbContextOptionsBuilder builder, string? connectionString)
        {
            builder.UsePostgreSql(connectionString, x => x.MigrationsAssembly(GetType().Assembly.FullName));
        }
    }
}
