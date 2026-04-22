using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using NPoco;
using NPoco.DatabaseTypes;
using NPoco.Expressions;
using Our.Umbraco.PostgreSql.Extensions;

namespace Our.Umbraco.PostgreSql.Services;

/// <summary>
/// Custom PostgreSQL database type that handles Umbraco-specific quirks.
/// </summary>
/// <remarks>
/// <para>
/// Umbraco's DTOs use "ID" as primary key names, but PostgreSQL is case-sensitive
/// when identifiers are quoted. This type normalizes primary key names to lowercase
/// to ensure compatibility with PostgreSQL's identifier handling.
/// </para>
/// <para>
/// This also handles DateTime conversion to UTC for PostgreSQL's TIMESTAMPTZ columns.
/// </para>
/// <para>
/// This eliminates the need for custom UmbracoDatabase and UmbracoDatabaseFactory implementations.
/// </para>
/// </remarks>
public class UmbracoPostgreSQLDatabaseType(IPackagesService packagesService) : DatabaseType
{
    #region PostgreSQL-specific overrides

    /// <inheritdoc />
    public override ISqlExpression<T> ExpressionVisitor<T>(IDatabase db, PocoData pocoData, bool prefixTableName)
    {
        return new PostgreSQLExpression<T>(db, pocoData, prefixTableName);
    }

    /// <inheritdoc />
    public override object MapParameterValue(object value)
    {
        // Don't map bools to ints in PostgreSQL
        if (value is bool)
        {
            return value;
        }

        // Convert DateTime to UTC for PostgreSQL TIMESTAMPTZ compatibility
        if (value is DateTime dt)
        {
            return dt.Kind switch
            {
                DateTimeKind.Unspecified => dt.ToLocalTime().ToUniversalTime(),
                DateTimeKind.Local => dt.ToUniversalTime(),
                _ => dt // Already UTC
            };
        }

        return base.MapParameterValue(value);
    }

    /// <inheritdoc />
    public override string EscapeSqlIdentifier(string str)
    {
        return Constants.EscapeTableColumAliasNames ? string.Format("\"{0}\"", str) : str;
    }

    /// <inheritdoc />
    public override string GetParameterPrefix(string connectionString)
    {
        return "@p";
    }

    /// <inheritdoc />
    public override string GetProviderName()
    {
        return "Npgsql2";
    }

    #endregion

    #region Umbraco-specific fixes

    /// <summary>
    /// Normalizes the primary key name to lowercase if it is "ID" to handle PostgreSQL case sensitivity.
    /// </summary>
    private static string? NormalizePrimaryKeyName(string? primaryKeyName)
    {
        if (string.IsNullOrEmpty(primaryKeyName))
        {
            return null;
        }

        // Composite keys (containing comma) should return null to skip RETURNING clause
        if (primaryKeyName.Contains(','))
        {
            return null;
        }

        // Normalize "ID" to "id" for case-sensitive PostgreSQL
        if (string.Equals(primaryKeyName, "ID", StringComparison.OrdinalIgnoreCase))
        {
            return "id";
        }

        return primaryKeyName;
    }

    private void AdjustSqlInsertCommandText(DbCommand cmd, string primaryKeyName)
    {
        cmd.CommandText += $" returning {EscapeSqlIdentifier(primaryKeyName)} as id";
    }

    /// <inheritdoc />
    public override object ExecuteInsert<T>(IDatabase db, DbCommand cmd, string primaryKeyName, bool useOutputClause, T poco, object[] args)
    {
        var normalizedPrimaryKey = NormalizePrimaryKeyName(primaryKeyName);

        if (normalizedPrimaryKey != null)
        {
            AdjustSqlInsertCommandText(cmd.FixCommanText(packagesService), normalizedPrimaryKey);
            return ((IDatabaseHelpers)db).ExecuteScalarHelper(cmd.FixCommanText(packagesService));
        }

        var fixedCmd = cmd.FixCommanText(packagesService);

        ((IDatabaseHelpers)db).ExecuteNonQueryHelper(fixedCmd);
        return -1;
    }

    /// <inheritdoc />
    public override async Task<object> ExecuteInsertAsync<T>(IDatabase db, DbCommand cmd, string primaryKeyName, bool useOutputClause, T poco, object[] args, CancellationToken cancellationToken = default)
    {
        var normalizedPrimaryKey = NormalizePrimaryKeyName(primaryKeyName);

        if (normalizedPrimaryKey != null)
        {
            AdjustSqlInsertCommandText(cmd, normalizedPrimaryKey);
            return await ((IDatabaseHelpers)db).ExecuteScalarHelperAsync(cmd.FixCommanText(packagesService), cancellationToken).ConfigureAwait(false);
        }

        await ((IDatabaseHelpers)db).ExecuteNonQueryHelperAsync(cmd.FixCommanText(packagesService), cancellationToken).ConfigureAwait(false);
        return -1;
    }

    public override void PreExecute(DbCommand cmd)
    {
        var fixedCmd = cmd.FixCommanText(packagesService);

        base.PreExecute(fixedCmd);
    }

    public override string FormatCommand(DbCommand cmd)
    {
        var fixedCmd = cmd.FixCommanText(packagesService);

        var rVal = base.FormatCommand(fixedCmd);

        return rVal;
    }
    public override Task<DbDataReader> ExecuteReaderAsync(IDatabase database, DbCommand cmd, CancellationToken cancellationToken = default)
    {
        var fixedCmd = cmd.FixCommanText(packagesService);

        return base.ExecuteReaderAsync(database, fixedCmd, cancellationToken);
    }
    #endregion
}
