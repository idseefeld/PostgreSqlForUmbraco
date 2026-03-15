using Umbraco.Cms.Core.Packaging;

namespace Our.Umbraco.PostgreSql.Migrations.versions;

public class PostgreSqlMigrationPlan : PackageMigrationPlan
{
    public PostgreSqlMigrationPlan()
        : base("Our.Umbraco.PostgreSql") // packageName = planName
    {
    }

    protected override void DefinePlan()
    {
        // Jeder To<>()-Aufruf registriert einen Migrationsschritt.
        // Die GUID ist der eindeutige State-Identifier für diesen Schritt.
        // Neue Versionen werden einfach unten angehängt.

        // v1.0.0 – Initiales Schema
        To<V_17_3.InitialPostgreSqlMigration>("{A1B2C3D4-0001-0001-0001-000000000001}"); // see table umbracoKeyValue

        // v1.1.0 – Neue Spalte hinzufügen
        // To<V_17_3_1.AddStatusColumnMigration>("{A1B2C3D4-0001-0001-0001-000000000002}");
    }
}
