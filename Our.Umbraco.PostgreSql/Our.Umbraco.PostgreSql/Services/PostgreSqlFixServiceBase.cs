using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Our.Umbraco.PostgreSql.Services
{
    public abstract class PostgreSqlFixServiceBase : IPostgreSqlFixService
    {
        public abstract bool FixCommanText(DbCommand cmd);

        public static string GetTimeZone(string timeZone = Constants.DefaultTimeZone)
        {
            var tz = "Europe/Berlin";

            if (timeZone.StartsWith("Central European Standard Time", StringComparison.OrdinalIgnoreCase))
            {
                tz = "Europe/Prague";
            }

            return $"AT TIME ZONE '{tz}' AT TIME ZONE 'UTC'";
        }

        public virtual Func<object, object>? GetParameterConverter(DbCommand dbCommand, Type sourceType) => null;

        public virtual void InterceptCommandExecuting(DbCommand cmd)
        { }
    }
}
