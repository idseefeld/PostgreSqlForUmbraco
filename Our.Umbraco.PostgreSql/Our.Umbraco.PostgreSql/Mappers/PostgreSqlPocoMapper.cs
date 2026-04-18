using NPoco;
using Our.Umbraco.PostgreSql.Services;
using System;
using System.Data.Common;
using System.Reflection;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql.Mappers
{
    public class PostgreSqlPocoMapper(IEnumerable<IPostgreSqlFixService> fixServices) : DefaultMapper
    {
        public override Func<object, object> GetFromDbConverter(MemberInfo destMemberInfo, Type sourceType)
        {
            var destName = destMemberInfo.DeclaringType?.Name;
            var sourceName = sourceType.Name;

            return base.GetFromDbConverter(destMemberInfo, sourceType);
        }

        public override Func<object, object?> GetFromDbConverter(Type destType, Type sourceType)
        {
            var destName = destType.Name;
            var sourceName = sourceType.Name;

            if (destType == typeof(long))
            {
                return value =>
                {
                    var result = long.Parse($"{value}");
                    return result;
                };
            }

            if (destType == typeof(Guid))
            {
                return value =>
                {
                    var result = Guid.Parse($"{value}");
                    return result;
                };
            }

            if (destType == typeof(Guid?))
            {
                return value =>
                {
                    if (Guid.TryParse($"{value}", out Guid result))
                    {
                        return result;
                    }

                    return default(Guid?);
                };
            }


            if (IsDateOnlyType(destType))
            {
                return value => ConvertToDateOnly(value, IsNullableType(destType));
            }

            if (IsTimeOnlyType(destType))
            {
                return value => ConvertToTimeOnly(value, IsNullableType(destType));
            }

            if (destType == typeof(DateTime))
            {
                return value =>
                {
                    if (value is DateTime result)
                    {
                        return result;
                    }

                    return default(DateTime);
                };
            }

            if (destType == typeof(DateTime?))
            {
                return value =>
                {
                    if (value == null)
                    {
                        return default(DateTime?);
                    }

                    if (value is DateTime result)
                    {
                        return result;
                    }

                    return value;
                };
            }

            return base.GetFromDbConverter(destType, sourceType);
        }

        private static bool IsDateOnlyType(Type type) =>
            type == typeof(DateOnly) || type == typeof(DateOnly?);

        private static bool IsTimeOnlyType(Type type) =>
            type == typeof(TimeOnly) || type == typeof(TimeOnly?);

        private static bool IsNullableType(Type type) =>
            Nullable.GetUnderlyingType(type) != null;

        private static object? ConvertToDateOnly(object? value, bool isNullable)
        {
            if (value is null)
            {
                return isNullable ? null : default(DateOnly);
            }

            if (value is DateTime dt)
            {
                return DateOnly.FromDateTime(dt);
            }

            return DateOnly.Parse(value.ToString()!);
        }

        private static object? ConvertToTimeOnly(object? value, bool isNullable)
        {
            if (value is null)
            {
                return isNullable ? null : default(TimeOnly);
            }

            if (value is DateTime dt)
            {
                return TimeOnly.FromDateTime(dt);
            }

            return TimeOnly.Parse(value.ToString()!);
        }

        public override Func<object, object> GetToDbConverter(Type destType, MemberInfo sourceMemberInfo)
        {
            var destName = destType.Name;
            var sourceName = sourceMemberInfo.Name;

            var rVal = base.GetToDbConverter(destType, sourceMemberInfo);
            if (sourceMemberInfo.Name.InvariantContains("date"))
            {

            }

            return rVal;
        }

        public override Func<object, object> GetParameterConverter(DbCommand dbCommand, Type sourceType)
        {
            var sourceName = sourceType.Name;

            Func<object, object> rVal = base.GetParameterConverter(dbCommand, sourceType);
            if (dbCommand.CommandText != null && dbCommand.CommandText.Contains("parentID"))
            {
                dbCommand.CommandText.Replace("parentID", "parentId");
            }

            if (rVal == null)
            {
                foreach (var fixService in fixServices)
                {
                    rVal = fixService.GetParameterConverter(dbCommand, sourceType);
                    if (rVal != null)
                    {
                        break;
                    }
                }
            }

            return rVal ?? (value =>
            {
                if (value is DateTime dt && dt.Kind != DateTimeKind.Utc)
                {
                    // PostgreSQL Npgsql expects DateTime to be in UTC
                    DateTime rVas = dt.Kind == DateTimeKind.Local
                        ? dt.ToUniversalTime()
                        : dt.ToLocalTime().ToUniversalTime();
                    return rVas;
                }
                return value;
            });
        }
    }
}
