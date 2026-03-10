using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Cms.Core.Configuration.Models;

namespace Our.Umbraco.PostgreSql
{
    [UmbracoOptions(Constants.Configuration)]
    public class PostgreSqlOptions
    {
        public bool EscapeTableColumAliasNames { get; set; } = false;
    }
}
