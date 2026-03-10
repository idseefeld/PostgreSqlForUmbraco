using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace Our.Umbraco.PostgreSql.EFCore;

public class PostgreSqlEFCoreComposer : IComposer
{
    public new void Compose(IUmbracoBuilder builder)
    {
        builder
            .AddUmbracoPostgreSqlEFCoreSupport();
    }
}
