using Microsoft.Extensions.Logging;
using Our.Umbraco.PostgreSql.Migrations.Dtos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Our.Umbraco.PostgreSql.Migrations.versions.V_17_3_1;

public class AddStatusColumnMigration(IMigrationContext context, ILogger<AddStatusColumnMigration> logger) : AsyncMigrationBase(context)
{
    protected override async Task MigrateAsync()
    {
        if (DatabaseType != NPoco.DatabaseType.PostgreSQL 
            || (TableExists("myPackageTable") && ColumnExists("myPackageTable", "status")))
        {
            return;
        }

        AddColumn<MyPackageTableDto>("myPackageTable", "status");
        logger.LogInformation("Migration 'AddStatusColumnMigration' completed successfully.");
    }
}
