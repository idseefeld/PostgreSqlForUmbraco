using Our.Umbraco.PostgreSql.Interceptors;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Migrations.Notifications;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql;

/// <summary>
///     Automatically adds SQL Server support to Umbraco when this project is referenced.
/// </summary>
public class PostgreSqlComposer : IComposer
{
    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder)
    {
        builder
            .AddUmbracoPostgreSqlSupport();
    }
}
