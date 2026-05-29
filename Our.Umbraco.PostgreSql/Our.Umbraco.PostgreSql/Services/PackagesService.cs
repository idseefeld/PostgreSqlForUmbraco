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
        private readonly SemVersion _minRequiredCoreVersion = new SemVersion(18, 0, 2);

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

                // version 18.0.0-rc
                cmd.CommandText = cmd.CommandText
                        .Replace(".\"Text\"", ".\"text\"");

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
