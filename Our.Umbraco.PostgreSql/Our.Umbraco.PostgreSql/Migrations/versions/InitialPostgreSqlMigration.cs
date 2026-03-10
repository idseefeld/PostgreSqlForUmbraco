using Our.Umbraco.PostgreSql.Migrations.Dtos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Our.Umbraco.PostgreSql.Migrations.versions;

public class InitialPostgreSqlMigration(IMigrationContext context) : AsyncMigrationBase(context)
{
    protected override async Task MigrateAsync()
    {
        if (!TableExists("myPackageTable"))
        {
            // Create.Table<MyPackageTableDto>().Do();
        }
    }
}
