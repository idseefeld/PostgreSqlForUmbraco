using Our.Umbraco.PostgreSql.Migrations.Dtos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Our.Umbraco.PostgreSql.Migrations.versions;

public class AddStatusColumnMigration(IMigrationContext context) : AsyncMigrationBase(context)
{
    protected override async Task MigrateAsync()
    {
        if (TableExists("myPackageTable") && !ColumnExists("myPackageTable", "status"))
        {
            AddColumn<MyPackageTableDto>("myPackageTable", "status");
        }
    }
}
