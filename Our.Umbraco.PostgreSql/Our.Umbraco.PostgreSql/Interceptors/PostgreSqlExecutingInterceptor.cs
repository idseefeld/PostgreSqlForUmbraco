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
        command.FixCommanText(packagesService);
    }

    /// <summary>
    /// Called after execution (both for readers and scalars).
    /// </summary>
    public void OnExecutedCommand(IDatabase database, DbCommand command)
    {
        // Place for diagnostics or lightweight metrics
    }
}
