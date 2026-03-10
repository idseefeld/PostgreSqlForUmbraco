using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Our.Umbraco.PostgreSql.Mappers
{
    public class PostgreSqlGuidScalarMapper : ScalarMapper<Guid>
    {
        protected override Guid Map(object? value)
        {
            if (value is null || value == DBNull.Value)
            {
                return default;
            }

            try
            {
                return Guid.Parse($"{value}");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class PostgreSqlDateTimeScalarMapper : ScalarMapper<DateTime>
    {
        protected override DateTime Map(object value)
        {
            if (value is null || value == DBNull.Value)
                return default;

            if (value is DateTime dt)
                return dt;

            // PostgreSQL Npgsql returns timestamp as DateTime or string, depending on mapping
            if (value is string s && DateTime.TryParse(s, out var parsed))
                return parsed;

            throw new InvalidCastException($"Cannot convert value '{value}' of type '{value.GetType()}' to DateTime.");
        }
    }

    public class PostgreSqlLongScalarMapper : ScalarMapper<long>
    {
        protected override long Map(object? value)
        {
            if (value is null || value == DBNull.Value)
            {
                return 0;
            }

            long rVal = 0;
            try
            {
                rVal = long.Parse($"{value}");
            }
            catch (Exception)
            {
                throw;
            }

            return rVal;
        }
    }
}
