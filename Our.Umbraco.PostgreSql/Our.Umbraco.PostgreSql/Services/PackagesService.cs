using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Umbraco;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql.Services
{
    public class PackagesService : IPackagesService
    {
        private readonly IList<IPostgreSqlFixService> _fixPackageServices;
        private readonly ILogger<PackagesService> _logger;
        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();

        public PackagesService(ILogger<PackagesService> logger, IEnumerable<IPostgreSqlFixService> fixPackageServices)
        {
            _logger = logger;
            _fixPackageServices = fixPackageServices.ToList();

            if (_fixPackageServices.Count == 0)
            {
                _logger.LogInformation("No PostgreSQL package fix service available.");
            }
        }

        public DbCommand FixCommanText(DbCommand cmd)
        {
            var oldCommandText = cmd.CommandText;

            if (!FixCommandInternal(cmd))
            {
                foreach (IPostgreSqlFixService fix in _fixPackageServices)
                {
                    if (fix.InterceptCommandExecuting(cmd))
                    {
                        continue;
                    }
                }
            }

            if (cmd.CommandText != oldCommandText)
            {
                _logger.LogWarning("Umbraco.Forms fixes for PostgreSQL original CommandText: {OldCommandText} converted into: {NewCommandText}", oldCommandText, cmd.CommandText);
            }

            return cmd;
        }

        private bool FixCommandInternal(DbCommand cmd)
        {
            if (cmd.CommandText.Contains("["))
            {
                cmd.CommandText = cmd.CommandText
                    .Replace("[", "\"")
                    .Replace("]", "\"")
                    .Replace("CAST(NULL AS nvarchar(255))", "NULL")
                    .Replace("CAST(NULL AS datetime)", "NULL::TIMESTAMPTZ")
                    .Replace("CAST(NULL AS uniqueidentifier)", "NULL::UUID")
                    .Replace("IsA", "isA")
                    .Replace("IsL", "isL")
                    .Replace("Last", "last");
                return true;
            }
            else if (cmd.CommandText.Equals("\r\nUPDATE umbracoPropertyData\r\nSET textValue = varcharValue, varcharValue = NULL\r\nWHERE propertyTypeId IN (\r\n    SELECT id\r\n    FROM cmsPropertyType\r\n    WHERE dataTypeId IN (\r\n        SELECT nodeId\r\n        FROM umbracoDataType\r\n        WHERE propertyEditorAlias = 'Umbraco.Label'\r\n        AND dbType = 'Ntext'\r\n    )\r\n)\r\nAND varcharValue IS NOT NULL"))
            {
                cmd.CommandText = "UPDATE \"umbracoPropertyData\" SET \"textValue\" = \"varcharValue\", \"varcharValue\" = NULL WHERE \"propertyTypeId\" IN (SELECT \"id\" FROM \"cmsPropertyType\" WHERE \"dataTypeId\" IN (SELECT \"nodeId\" FROM \"umbracoDataType\" WHERE \"propertyEditorAlias\" = 'Umbraco.Label' AND \"dbType\" = 'Ntext')) AND \"varcharValue\" IS NOT NULL";
                return true;
            }

            return false;
        }

        public void InterceptCommandExecuting(DbCommand cmd)
        {
            foreach (IPostgreSqlFixService fix in _fixPackageServices)
            {
                fix.InterceptCommandExecuting(cmd);
            }
        }
    }
}
