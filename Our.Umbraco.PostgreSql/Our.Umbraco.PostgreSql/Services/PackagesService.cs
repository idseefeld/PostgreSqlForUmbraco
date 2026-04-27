using System.Data.Common;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Semver;
using Umbraco.Cms.Core.Services;

namespace Our.Umbraco.PostgreSql.Services
{
    public class PackagesService : IPackagesService
    {
        private readonly SemVersion _minRequiredCoreVersion = new SemVersion(17, 4, 0);

        private readonly IServerInformationService _serverInformationService;
        private readonly IList<IPostgreSqlFixService> _fixPackageServices;
        private readonly ILogger<PackagesService> _logger;

        private bool _hasAllCoreFixes = false;

        private readonly Lock _lock = new();

        public PackagesService(ILogger<PackagesService> logger, IEnumerable<IPostgreSqlFixService> fixPackageServices, IServerInformationService serverInformationService)
        {
            _logger = logger;
            _fixPackageServices = fixPackageServices.ToList();

            _serverInformationService = serverInformationService;

            if (_fixPackageServices.Count == 0)
            {
                _logger.LogInformation("No PostgreSQL package fix service available.");
            }
        }

        public DbCommand FixCommandText(DbCommand cmd)
        {
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

            return cmd;
        }

        private bool MinUmbracoVersionRequired(SemVersion assumedVersion)
        {
            if (_hasAllCoreFixes)
            {
                return true;
            }

            ServerInformation serverInfo = _serverInformationService.GetServerInformation();
            lock (_lock)
            {
                if (serverInfo != null && serverInfo.SemVersion >= assumedVersion)
                {
                    _hasAllCoreFixes = true;
                }
            }

            return _hasAllCoreFixes;
        }

        private bool FixCommandInternal(DbCommand cmd)
        {
            var cmdFixed = false;
            if (MinUmbracoVersionRequired(_minRequiredCoreVersion))
            {
                return cmdFixed;
            }

            lock (_lock)
            {
                var oldCommandText = cmd.CommandText;

                if (cmd.CommandText.Contains('['))
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

                    cmdFixed = true;
                }
                else
                {
                    const string umbracoPropertyDataSql = "\r\nUPDATE umbracoPropertyData\r\nSET textValue = varcharValue, varcharValue = NULL\r\nWHERE propertyTypeId IN (\r\n    SELECT id\r\n    FROM cmsPropertyType\r\n    WHERE dataTypeId IN (\r\n        SELECT nodeId\r\n        FROM umbracoDataType\r\n        WHERE propertyEditorAlias = 'Umbraco.Label'\r\n        AND dbType = 'Ntext'\r\n    )\r\n)\r\nAND varcharValue IS NOT NULL";

                    if (cmd.CommandText.Equals(umbracoPropertyDataSql))
                    {
                        cmd.CommandText = "UPDATE \"umbracoPropertyData\" SET \"textValue\" = \"varcharValue\", \"varcharValue\" = NULL WHERE \"propertyTypeId\" IN (SELECT \"id\" FROM \"cmsPropertyType\" WHERE \"dataTypeId\" IN (SELECT \"nodeId\" FROM \"umbracoDataType\" WHERE \"propertyEditorAlias\" = 'Umbraco.Label' AND \"dbType\" = 'Ntext')) AND \"varcharValue\" IS NOT NULL";
                        cmdFixed = true;
                    }
                }

                if (cmdFixed)
                {
                    _logger.LogWarning("Fixes for PostgreSQL applied - original CommandText: {OriginalCommandText} converted into: {ConvertedCommandText}", oldCommandText, cmd.CommandText);
                }

                return cmdFixed;
            } // end lock
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
