using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Extensions.Logging;
using NPoco;
using Umbraco.Cms.Infrastructure.Migrations.Install;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Our.Umbraco.PostgreSql.Services
{
    public class PostgreSqlDatabase : UmbracoDatabase
    {
        public PostgreSqlDatabase(string connectionString, ISqlContext sqlContext, DbProviderFactory provider, ILogger<UmbracoDatabase> logger, IBulkSqlInsertProvider? bulkSqlInsertProvider, DatabaseSchemaCreatorFactory databaseSchemaCreatorFactory, IEnumerable<IMapper>? mapperCollection = null)
            : base(connectionString, sqlContext, provider, logger, bulkSqlInsertProvider, databaseSchemaCreatorFactory, mapperCollection)
        {
        }

        public new int Execute(string sql, CommandType commandType, params object[] args)
        {

            return base.Execute(sql, commandType, args);
        }
    }
}
