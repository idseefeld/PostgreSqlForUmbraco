using Our.Umbraco.PostgreSql.Mappers;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Our.Umbraco.PostgreSql.Services
{
    public class PostgreSqlSpecificMapperFactory : IProviderSpecificMapperFactory
    {
        /// <inheritdoc />
        public string ProviderName => Constants.ProviderName;

        /// <inheritdoc />
        public NPocoMapperCollection Mappers => new(() => [new PostgreSqlPocoMapper()]);
    }
}
