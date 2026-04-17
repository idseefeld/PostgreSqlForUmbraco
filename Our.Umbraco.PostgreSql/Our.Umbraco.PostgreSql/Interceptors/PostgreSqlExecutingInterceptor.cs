using NPoco;
using Umbraco.Cms.Infrastructure.Persistence;
using System.Data.Common;
using Our.Umbraco.PostgreSql.Extensions;
using Our.Umbraco.PostgreSql.Services;

namespace Our.Umbraco.PostgreSql.Interceptors;

/// <summary>
/// Provider-specific interceptor to customize PostgreSQL command execution.
/// </summary>
public class PostgreSqlExecutingInterceptor(IPackagesService packagesService) : IProviderSpecificExecutingInterceptor
{
    public string ProviderName => Constants.ProviderName;

    /// <summary>
    /// Called before NPoco executes a DbCommand.
    /// </summary>
    public void OnExecutingCommand(IDatabase database, DbCommand command)
    {
        // Place for changes e.g. of the command.CommandText
        if (command.CommandText.Contains(" NONCLUSTERED INDEX "))
        {
            // Example of a specific fix for a known issue with the covering index creation command
            command.CommandText = command.CommandText.Replace(" NONCLUSTERED INDEX ", " INDEX ");
        }
        else if (command.CommandText.Equals("DROP INDEX \"IX_UFRecords_Form_Created\" ON \"UFRecords\""))
        {
            // Example of a specific fix for a known issue with dropping the index
            command.CommandText = "DROP INDEX IF EXISTS \"IX_UFRecords_Form_Created\"";
        }
        else if (command.CommandText.StartsWith("DELETE FROM UFAnalyticsProcessedDates WHERE [Date] < @0")
            || command.CommandText.StartsWith("DELETE FROM UFAnalyticsProcessedDates WHERE \"Date\" < @p0"))
        {
            // Example of a specific fix for a known issue with creating the index
            command.CommandText = "DELETE FROM \"UFAnalyticsProcessedDates\" WHERE \"Date\" < @p0";
        }
    }

    /// <summary>
    /// Called after execution (both for readers and scalars).
    /// </summary>
    public void OnExecutedCommand(IDatabase database, DbCommand command)
    {
        // Place for diagnostics or lightweight metrics
    }
}
