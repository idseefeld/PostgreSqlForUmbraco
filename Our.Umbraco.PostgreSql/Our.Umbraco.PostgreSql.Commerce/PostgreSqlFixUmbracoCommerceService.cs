using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Our.Umbraco.PostgreSql.Services;

namespace Our.Umbraco.PostgreSql.Commerce
{
    public class PostgreSqlFixUmbracoCommerceService : PostgreSqlFixServiceBase
    {
        private readonly Lock _lock = new();
        private bool FixCommandInternal(DbCommand cmd)
        {
            var success = true;

            if (!cmd.CommandText.Contains("Umbraco.Commerce"))
            {
                return success;
            }

            lock (_lock)
            {
                var pos = 0;
                switch (cmd.CommandText)
                {
                    case "UPDATE [umbracoDataType]\r\nSET [config] = JSON_MODIFY(JSON_MODIFY([config], '$.additionalDecimalPlaces', CAST(JSON_VALUE([config],'$.fraction') AS INT) - 2), '$.fraction', NULL)\r\nWHERE [propertyEditorAlias] = 'Umbraco.Commerce.Price'\r\nAND JSON_VALUE([config], '$.fraction') IS NOT NULL;\r\n":
                        cmd.CommandText = "UPDATE \"umbracoDataType\" SET \"config\" = jsonb_set(\"config\" - 'fraction', '{additionalDecimalPlaces}', to_jsonb((\"config\"->>'fraction')::integer - 2)) WHERE \"propertyEditorAlias\" = 'Umbraco.Commerce.Price' AND \"config\"->>'fraction' IS NOT NULL;";
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
