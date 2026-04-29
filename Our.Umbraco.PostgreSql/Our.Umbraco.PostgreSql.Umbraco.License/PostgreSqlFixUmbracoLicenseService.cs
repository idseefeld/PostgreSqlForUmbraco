using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Our.Umbraco.PostgreSql.Extensions;
using Our.Umbraco.PostgreSql.Services;

namespace Our.Umbraco.PostgreSql.Umbraco.License
{
    /// <summary>
    /// Provides functionality to correct Umbraco.License SQL statements and validation timestamp updates in PostgreSQL database commands.
    /// </summary>
    public class PostgreSqlFixUmbracoLicenseService : PostgreSqlFixServiceBase
    {
        private readonly Lock _lock = new();

        private bool FixCommandInternal(DbCommand cmd)
        {
            var success = true;

            if (!cmd.CommandText.StartsWith("UPDATE umbracoProductLicenseValidationStatus"))
            {
                return success;
            }

            lock (_lock)
            {
                switch (cmd.CommandText)
                {
                    case "UPDATE umbracoProductLicenseValidationStatus SET LastValidatedOn = LastValidatedOn AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"umbracoProductLicenseValidationStatus\" SET \"LastValidatedOn\" = \"LastValidatedOn\" {GetTimeZone()}";
                        break;
                    case "UPDATE umbracoProductLicenseValidationStatus SET LastSuccessfullyValidatedOn = LastSuccessfullyValidatedOn AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"umbracoProductLicenseValidationStatus\" SET \"LastSuccessfullyValidatedOn\" = \"LastSuccessfullyValidatedOn\" {GetTimeZone()}";
                        break;
                    case "UPDATE umbracoProductLicenseValidationStatus SET ExpiresOn = ExpiresOn AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"umbracoProductLicenseValidationStatus\" SET \"ExpiresOn\" = \"ExpiresOn\" {GetTimeZone()}";
                        break;
                    default:
                        success = false;
                        break;
                }

                return success;
            } // end lock
        }

        public override bool InterceptCommandExecuting(DbCommand cmd)
        {
            var success = base.InterceptCommandExecuting(cmd);

            success = success && FixCommandInternal(cmd);

            return success;
        }
    }
}
