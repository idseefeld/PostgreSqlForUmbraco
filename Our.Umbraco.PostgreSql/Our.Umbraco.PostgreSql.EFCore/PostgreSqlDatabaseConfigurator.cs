// Copyright (c) Umbraco.
// See LICENSE for more details.

using Microsoft.EntityFrameworkCore;
using Our.Umbraco.PostgreSql.EFCore.Extensions;
using Umbraco.Cms.Infrastructure.Persistence.EFCore;

namespace Our.Umbraco.PostgreSql.EFCore;

public class PostgreSqlDatabaseConfigurator : IDatabaseConfigurator
{
    /// <inheritdoc />
    public bool CanHandle(string providerName)
        => string.Equals(providerName, Constants.ProviderName, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc />
    public void Configure(DbContextOptionsBuilder builder, string connectionString)
        => builder.UsePostgreSql(connectionString);
}
