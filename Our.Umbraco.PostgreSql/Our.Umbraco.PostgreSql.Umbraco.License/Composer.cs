using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Our.Umbraco.PostgreSql.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Our.Umbraco.PostgreSql.Umbraco.License;

/// <summary>
/// Adds fixes for Umbraco.Forms sql statements when using PostgreSQL as the database provider.
/// </summary>
public class Composer : IComposer
{
    /// <inheritdoc />
    public void Compose(IUmbracoBuilder builder) => builder.Services
        .TryAddEnumerable(ServiceDescriptor.Singleton<IPostgreSqlFixService, PostgreSqlFixUmbracoLicenseService>());
}
