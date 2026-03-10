using Our.Umbraco.PostgreSql.Migrations.versions;
using Umbraco.Cms.Core.Packaging;

namespace Our.Umbraco.PostgreSql.Migrations;

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
        To<InitialPostgreSqlMigration>("{A1B2C3D4-0001-0001-0001-000000000001}"); // see table umbracoKeyValue

        /*
        // v1.1.0 – Neue Spalte hinzufügen
        To<AddStatusColumnMigration>("{A1B2C3D4-0001-0001-0001-000000000002}");

        // v1.2.0 – Index hinzufügen
        To<AddStatusIndexMigration>("{6751328a-dc70-4bdd-a444-87f730e693e0}");

        // v1.3.0 – Neue Spalte hinzufügen
        To<DeleteIndexMigration>("{3ef4a8b9-f63c-437c-830b-7a4218f40d7b}");
        */
    }
}
