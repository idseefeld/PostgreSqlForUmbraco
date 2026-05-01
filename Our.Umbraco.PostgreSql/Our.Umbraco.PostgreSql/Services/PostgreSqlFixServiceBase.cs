using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Our.Umbraco.PostgreSql.Services
{
    public abstract class PostgreSqlFixServiceBase : IPostgreSqlFixService
    {
        public static string GetTimeZone(string timeZone = Constants.DefaultTimeZone)
        {
            // Use .NET's built-in Windows → IANA timezone conversion first, so any
            // Windows timezone name that arrives from a Forms migration SQL statement
            // is correctly mapped to a PostgreSQL-compatible IANA identifier.
            if (!string.IsNullOrWhiteSpace(timeZone)
                && TimeZoneInfo.TryConvertWindowsIdToIanaId(timeZone, out var ianaId)
                && !string.IsNullOrWhiteSpace(ianaId))
            {
                return $"AT TIME ZONE '{ianaId}' AT TIME ZONE 'UTC'";
            }

            // Fallback for legacy / edge cases where conversion is not available.
            var tz = "Europe/Berlin";

            if (timeZone.StartsWith("Central European Standard Time", StringComparison.OrdinalIgnoreCase))
            {
                tz = "Europe/Prague";
            }

            return $"AT TIME ZONE '{tz}' AT TIME ZONE 'UTC'";
        }

        public virtual Func<object, object>? GetParameterConverter(DbCommand dbCommand, Type sourceType) => null;

        // public virtual bool FixCommanText(DbCommand cmd) => true;

        public virtual bool InterceptCommandExecuting(DbCommand cmd) => true;
    }
}
