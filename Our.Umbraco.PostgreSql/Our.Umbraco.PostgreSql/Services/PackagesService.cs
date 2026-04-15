using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
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
            //var commandHash = oldCommandText.GenerateHash();
            //if (_cache.TryGetValue(commandHash, out string? cachedCmdText))
            //{
            //    cmd.CommandText = cachedCmdText ?? cmd.CommandText;
            //    return cmd;
            //}

            foreach (IPostgreSqlFixService fix in _fixPackageServices)
            {
                if (fix.FixCommanText(cmd))
                {
                    continue;
                }

                if (cmd.CommandText != oldCommandText)
                {
                    _logger.LogWarning("Umbraco.Forms fixes for PostgreSQL original CommandText: {OldCommandText} converted into: {NewCommandText}", oldCommandText, cmd.CommandText);
                    // _cache.Add(commandHash, cmd.CommandText);
                }
            }

            return cmd;
        }
    }
}
