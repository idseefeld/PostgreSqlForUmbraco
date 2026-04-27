using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Our.Umbraco.PostgreSql.EFCore.Services;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql.EFCore.Extensions
{
    public static class PostgreSqlDbContextExtensions
    {
        public static void UsePostgreSqlDatabaseProvider(this DbContextOptionsBuilder builder, IServiceProvider serviceProvider)
        {
            ConnectionStrings connectionStrings = serviceProvider.GetRequiredService<IOptionsMonitor<ConnectionStrings>>().CurrentValue;

            builder.UseNpgsql(connectionStrings.ConnectionString);
        }

        public static IServiceCollection AddPostgreSqlDatabaseContext<T>(
            this IServiceCollection services,
            Action<IServiceProvider, DbContextOptionsBuilder, string?, string?>? optionsAction,
            bool shareUmbracoConnection)
            where T : PostgreSqlDbContext
        {
            return services.AddUmbracoDbContext<T>(optionsAction, shareUmbracoConnection);
        }
    }
}
