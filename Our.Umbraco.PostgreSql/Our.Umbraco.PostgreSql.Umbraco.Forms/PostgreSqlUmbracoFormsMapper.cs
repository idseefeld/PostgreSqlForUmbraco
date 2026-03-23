using NPoco;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Our.Umbraco.PostgreSql.Umbraco.Forms
{
    public class PostgreSqlUmbracoFormsMapper : DefaultMapper
    {
        public override Func<object, object> GetParameterConverter(DbCommand dbCommand, Type sourceType)
        {
            var sourceName = sourceType.Name;
            Func<object, object> rVal = base.GetParameterConverter(dbCommand, sourceType);

            return rVal ?? (value =>
            {
                if (!dbCommand.CommandText.ContainsUfTableName())
                {
                    return value;
                }

                if (value is string str && Guid.TryParse(str, out Guid guidValue))
                {
                    return guidValue;
                }

                if (value is int i 
                    && dbCommand.CommandText.ContainsUfTableName("UFUserGroupSecurity") 
                    && bool.TryParse(i.ToString(), out bool boolValue))
                {
                    // !!!! nicht alle spalten sind boolean
                    return boolValue;
                }

                return value;
            });
        }
    }
}
