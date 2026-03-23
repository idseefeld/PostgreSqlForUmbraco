namespace Our.Umbraco.PostgreSql.Umbraco.Forms
{
    internal static class UmbracoFormsDefinitions
    {
        public static string[] UfTables = [
            "UFDataSource",
            "UFFolders",
            "UFForms",
            "UFPrevalueSource",
            "UFRecordAudit",
            "UFRecordDataDateTime",
            "UFRecordDataInteger",
            "UFRecordDataLongString",
            "UFRecordDataString",
            "UFRecordWorkflowAudit",
            "UFRecords",
            "UFUserFormSecurity",
            "UFUserGroupFormSecurity",
            "UFUserGroupSecurity",
            "UFWorkflows",
            ];

        public static bool ContainsUfTableName(this string sqlStatement, string tableName = null)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                foreach (var table in UfTables)
                {
                    if (sqlStatement.Contains(table, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (UfTables.Contains(tableName) && sqlStatement.Contains(tableName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        //public static string[] UfColumns = [
        //        "DataType",
        //        "FormGuid",
        //        "Id",
        //        "Key",
        //        "Name",
        //        "Value"
        //    ];
    }
}
