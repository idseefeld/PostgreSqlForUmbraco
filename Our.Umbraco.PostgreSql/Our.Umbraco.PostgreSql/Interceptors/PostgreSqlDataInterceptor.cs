using NPoco;
using Umbraco.Cms.Infrastructure.Persistence;
using Our.Umbraco.PostgreSql;

namespace Our.Umbraco.PostgreSql.Interceptors;

public class PostgreSqlDataInterceptor : IProviderSpecificDataInterceptor
{
    public string ProviderName => Constants.ProviderName;

    public bool OnInserting(IDatabase database, InsertContext insertContext) => true;

    public bool OnUpdating(IDatabase database, UpdateContext updateContext) => true;

    public bool OnDeleting(IDatabase database, DeleteContext deleteContext) => true;
}
