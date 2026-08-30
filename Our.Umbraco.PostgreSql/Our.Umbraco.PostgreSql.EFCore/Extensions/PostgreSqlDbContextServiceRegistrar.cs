// Copyright (c) Umbraco.
// See LICENSE for more details.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Our.Umbraco.PostgreSql.EFCore.Locking;
using Umbraco.Cms.Core.DistributedLocking;
using Umbraco.Cms.Infrastructure.Persistence.EFCore;

namespace Our.Umbraco.PostgreSql.EFCore.Extensions;

/// <summary>
/// Registers SQL Server-specific services for a given <see cref="DbContext"/> type.
/// </summary>
public class PostgreSqlDbContextServiceRegistrar : IDbContextServiceRegistrar
{
    /// <inheritdoc />
    public bool CanHandle(string providerName)
        => string.Equals(providerName, Constants.ProviderName, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public void RegisterServices<TContext>(IServiceCollection services)
        where TContext : DbContext
        => services.AddSingleton<IDistributedLockingMechanism, PostgreSqlEFCoreDistributedLockingMechanism<TContext>>();
}
