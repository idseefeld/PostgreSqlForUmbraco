using Microsoft.Extensions.Logging;
using Our.Umbraco.PostgreSql.Migrations.Dtos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Our.Umbraco.PostgreSql.Migrations.versions.V_17_3;

public class InitialPostgreSqlMigration(IMigrationContext context, ILogger<InitialPostgreSqlMigration> logger) : AsyncMigrationBase(context)
{
    protected override async Task MigrateAsync()
    {
        if (DatabaseType == NPoco.DatabaseType.PostgreSQL && !TableExists("myPackageTable"))
        {
            Create.Table<MyPackageTableDto>().Do();
        }
    }
}
