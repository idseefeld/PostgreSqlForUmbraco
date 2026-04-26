using System.Data.Common;
using Our.Umbraco.PostgreSql.Services;

namespace Our.Umbraco.PostgreSql.Extensions
{
    internal static class DbCommandExtensions
    {
        public static DbCommand FixCommanText(this DbCommand cmd, IPackagesService? packagesFixService)
        {
            if (string.IsNullOrEmpty(cmd.CommandText))
            {
                return cmd;
            }

            packagesFixService?.FixCommanText(cmd);

            if (cmd.CommandText.Contains('['))
            {
                cmd.CommandText = cmd.CommandText.Replace("[", "\"").Replace("]", "\"");
            }

            return cmd;
        }
    }
}
