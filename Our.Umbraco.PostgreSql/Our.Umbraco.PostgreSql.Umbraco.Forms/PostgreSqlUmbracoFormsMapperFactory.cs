using Our.Umbraco.PostgreSql.Mappers;
using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Our.Umbraco.PostgreSql.Umbraco.Forms
{
    public class PostgreSqlUmbracoFormsMapperFactory : IProviderSpecificMapperFactory
    {
        /// <inheritdoc />
        public string ProviderName => Constants.ProviderName + ".Umbraco.Forms";

        /// <inheritdoc />
        public NPocoMapperCollection Mappers => new(() => [new PostgreSqlUmbracoFormsMapper()]);
    }
}
