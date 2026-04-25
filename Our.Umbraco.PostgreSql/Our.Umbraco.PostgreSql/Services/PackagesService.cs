using System.Data.Common;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Semver;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql.Services
{
    public class PackagesService : IPackagesService
    {
        private readonly IServerInformationService _serverInformationService;
        private readonly IPackageManifestService _packageManifestService;

        private readonly IList<IPostgreSqlFixService> _fixPackageServices;
        private readonly ILogger<PackagesService> _logger;

        private bool _hasAllCoreFixes = false;

        private bool _containsSquareBrackets = false;
        private bool _updatesUmbracoPropertyData = false;


        public PackagesService(ILogger<PackagesService> logger, IEnumerable<IPostgreSqlFixService> fixPackageServices, IPackageManifestService packageManifestService, IServerInformationService serverInformationService)
        {
            _logger = logger;
            _fixPackageServices = fixPackageServices.ToList();

            _packageManifestService = packageManifestService;
            _serverInformationService = serverInformationService;

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

        private bool HasAllCoreFixes()
        {
            if (_hasAllCoreFixes)
            {
                return true;
            }

            ServerInformation serverInfo = _serverInformationService.GetServerInformation();
            if (serverInfo != null && serverInfo.SemVersion >= new SemVersion(17, 4, 0))
            {
                _hasAllCoreFixes = true;
            }

            return _hasAllCoreFixes;
        }

        private bool FixCommandInternal(DbCommand cmd)
        {
            if (_containsSquareBrackets || cmd.CommandText.Contains("["))
            {
                _containsSquareBrackets = true;

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
            else if (!HasAllCoreFixes())
            {
                const string umbracoPropertyDataSql = "\r\nUPDATE umbracoPropertyData\r\nSET textValue = varcharValue, varcharValue = NULL\r\nWHERE propertyTypeId IN (\r\n    SELECT id\r\n    FROM cmsPropertyType\r\n    WHERE dataTypeId IN (\r\n        SELECT nodeId\r\n        FROM umbracoDataType\r\n        WHERE propertyEditorAlias = 'Umbraco.Label'\r\n        AND dbType = 'Ntext'\r\n    )\r\n)\r\nAND varcharValue IS NOT NULL";

                if (_updatesUmbracoPropertyData || cmd.CommandText.Equals(umbracoPropertyDataSql))
                {
                    _updatesUmbracoPropertyData = true;
                    cmd.CommandText = "UPDATE \"umbracoPropertyData\" SET \"textValue\" = \"varcharValue\", \"varcharValue\" = NULL WHERE \"propertyTypeId\" IN (SELECT \"id\" FROM \"cmsPropertyType\" WHERE \"dataTypeId\" IN (SELECT \"nodeId\" FROM \"umbracoDataType\" WHERE \"propertyEditorAlias\" = 'Umbraco.Label' AND \"dbType\" = 'Ntext')) AND \"varcharValue\" IS NOT NULL";
                    return true;
                }
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
