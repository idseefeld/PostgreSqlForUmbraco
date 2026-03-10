using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Our.Umbraco.PostgreSql.Interceptors
{
    public class PostgreSqlAddMiniProfilerInterceptor : IProviderSpecificInterceptor
    {
        public string ProviderName => Constants.ProviderName;
    }
}
