using Our.Umbraco.PostgreSql.Migrations.Dtos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Our.Umbraco.PostgreSql.Migrations.versions;

public class AddStatusIndexMigration(IMigrationContext context) : AsyncMigrationBase(context)
{
    protected override async Task MigrateAsync()
    {
        if (!IndexExists("IX_myPackageTable_status"))
        {
            CreateIndex<MyPackageTableDto>("IX_myPackageTable_status");
        }
    }
}
