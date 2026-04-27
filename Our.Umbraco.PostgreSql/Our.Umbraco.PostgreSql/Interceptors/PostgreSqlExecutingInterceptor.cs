using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using NPoco;
using Our.Umbraco.PostgreSql.Extensions;
using Our.Umbraco.PostgreSql.Services;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Our.Umbraco.PostgreSql.Interceptors;

/// <summary>
/// Provider-specific interceptor to customize PostgreSQL command execution.
/// </summary>
public class PostgreSqlExecutingInterceptor(IServiceProvider serviceProvider) : IProviderSpecificExecutingInterceptor
{
    public string ProviderName => Constants.ProviderName;

    // Resolved lazily on first use
    private IPackagesService? PackagesService => field ??= serviceProvider?.GetService<IPackagesService>();

    /// <summary>
    /// Called before NPoco executes a DbCommand.
    /// </summary>
    public void OnExecutingCommand(IDatabase database, DbCommand command)
    {
        PackagesService.InterceptCommandExecuting(command);
    }

    /// <summary>
    /// Called after execution (both for readers and scalars).
    /// </summary>
    public void OnExecutedCommand(IDatabase database, DbCommand command)
    {
        // Place for diagnostics or lightweight metrics
    }
}
