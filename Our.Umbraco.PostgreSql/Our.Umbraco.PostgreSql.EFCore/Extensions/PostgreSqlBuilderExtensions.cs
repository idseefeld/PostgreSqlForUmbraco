using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.PostgreSql.EFCore.Locking;
using Our.Umbraco.PostgreSql.EFCore.Services;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.DistributedLocking;
using Umbraco.Cms.Persistence.EFCore;
using Umbraco.Cms.Persistence.EFCore.Migrations;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql.EFCore.Extensions;

/// <summary>
/// Provides extension methods for configuring Umbraco to support PostgreSQL using Entity Framework Core.
/// </summary>
public static class PostgreSqlBuilderExtensions
{
    /// <summary>
    ///     Add required services for PostgreSQL support.
    /// </summary>
    public static IUmbracoBuilder AddUmbracoPostgreSqlEFCoreSupport(this IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IMigrationProvider, PostgreSqlMigrationProvider>();
        builder.Services.AddSingleton<IMigrationProviderSetup, PostgreSqlMigrationProviderSetup>();

        builder.Services.AddSingleton<IDistributedLockingMechanism, PostgreSqlEFCoreDistributedLockingMechanism<UmbracoDbContext>>();

        return builder;
    }
}
