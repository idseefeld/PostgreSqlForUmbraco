using System;
using System.Collections.Generic;
using System.Text;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Our.Umbraco.PostgreSql.Mappers
{
    public class PostgreSqlNullableGuidScalarMapper : ScalarMapper<Guid?>
    {
        protected override Guid? Map(object? value)
        {
            if (value is null || value == DBNull.Value)
            {
                return default;
            }

            try
            {
                return Guid.TryParse($"{value}", out Guid result)
                      ? result
                      : default(Guid?);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public class PostgreSqlNullableDateTimeScalarMapper : ScalarMapper<DateTime?>
    {
        protected override DateTime? Map(object value)
        {
            if (value is null || value == DBNull.Value)
            { return default; }

            if (value is DateTime dt)
            { return dt; }

            // PostgreSQL Npgsql returns timestamp as DateTime or string, depending on mapping
            if (value is string s && DateTime.TryParse(s, out DateTime parsed))
            { return parsed; }

            throw new InvalidCastException($"Cannot convert value '{value}' of type '{value.GetType()}' to DateTime.");
        }
    }

    public class PostgreSqlNullableLongScalarMapper : ScalarMapper<long?>
    {
        protected override long? Map(object? value)
        {
            if (value is null || value == DBNull.Value)
            {
                return default;
            }

            try
            {
                return long.TryParse($"{value}", out long result)
                      ? result
                      : default(long?);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
