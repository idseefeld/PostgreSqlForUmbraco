using System.Data.Common;
using Our.Umbraco.PostgreSql.Services;

namespace Our.Umbraco.PostgreSql.Extensions
{
    internal static class DbCommandExtensions
    {
        public static DbCommand FixCommandText(this DbCommand cmd, IPackagesService? packagesFixService)
        {
            if (string.IsNullOrEmpty(cmd.CommandText))
            {
                return cmd;
            }

            packagesFixService?.FixCommandText(cmd);

            return cmd;
        }
    }
}
