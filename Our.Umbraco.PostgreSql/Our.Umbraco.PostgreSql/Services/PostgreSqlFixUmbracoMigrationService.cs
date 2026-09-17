// Copyright (c) Umbraco.
// See LICENSE for more details.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using StackExchange.Profiling.Internal;
using PropertyEditors = Umbraco.Cms.Core.Constants.PropertyEditors;

namespace Our.Umbraco.PostgreSql.Services
{
    /// <summary>
    /// This fix is only necessary for Umbraco 18+ when upgrading form version 17.4+, which had a migration that incorrectly set the property editor alias for the Single Block editor.
    /// </summary>
    public class PostgreSqlFixUmbracoMigrationService : PostgreSqlFixServiceBase
    {
        private bool FixCommandInternal(DbCommand cmd)
        {
            var success = true;
            if (!cmd.CommandText.StartsWith("UPDATE umbracoDataType")
                && !cmd.CommandText.Contains("'Umb.PropertyEditorUi.BlockSingle'")
                )
            {
                return success;
            }

            switch (cmd.CommandText)
            {
                case "\r\nUPDATE umbracoDataType\r\nSET propertyEditorAlias = 'Umbraco.SingleBlock',\r\n    propertyEditorUiAlias = 'Umb.PropertyEditorUi.BlockSingle'\r\nWHERE nodeId IN (@p0)":
                    var inClause = string.Join(",", cmd.Parameters.Cast<DbParameter>().Select(p => p.Value));
                    cmd.CommandText = $"UPDATE \"umbracoDataType\" SET \"propertyEditorAlias\" = 'Umbraco.SingleBlock', \"propertyEditorUiAlias\" = 'Umb.PropertyEditorUi.BlockSingle' WHERE \"nodeId\" IN ({inClause})";
                    break;
                default:
                    success = false;
                    break;
            }
            return success;
        }

        public override bool InterceptCommandExecuting(DbCommand cmd)
        {
            var success = base.InterceptCommandExecuting(cmd);

            success = success && FixCommandInternal(cmd);

            return success;
        }
    }
}
