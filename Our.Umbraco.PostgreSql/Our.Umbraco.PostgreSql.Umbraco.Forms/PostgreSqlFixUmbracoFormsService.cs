using System.Data;
using System.Data.Common;
using Microsoft.Extensions.Logging;
using Our.Umbraco.PostgreSql.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql.Umbraco.Forms
{
    /// <summary>
    /// Provides functionality to correct Umbraco.Forms SQL statements and validation timestamp updates in PostgreSQL database commands.
    /// </summary>
    public class PostgreSqlFixUmbracoFormsService : PostgreSqlFixServiceBase
    {       
        public override bool FixCommanText(DbCommand cmd) => FixUmbracoFormsIssues(cmd);

        private bool FixUmbracoFormsIssues(DbCommand cmd)
        {
            var success = true;

            if (!(cmd.CommandText.Contains(" UF") || cmd.CommandText.Contains(" \"UF")))
            {
                return success;
            }

            var oldCommandText = cmd.CommandText;
            

            if (cmd.CommandText.StartsWith("SELECT "))
            {
                switch (cmd.CommandText)
                {
                    case "SELECT COUNT(*)\nFROM \"UFFolders\"\nWHERE (\"UFFolders\".\"ParentKey\" NOT IN (SELECT \"UFFolders\".\"Key\" AS \"Key\"\nFROM \"UFFolders\"))\nAND (ParentKey Is Not Null)":
                        cmd.CommandText = "SELECT COUNT(*)\nFROM \"UFFolders\"\nWHERE (\"UFFolders\".\"ParentKey\" NOT IN (SELECT \"UFFolders\".\"Key\" AS \"Key\"\nFROM \"UFFolders\"))\nAND (\"ParentKey\" Is Not Null)";
                        break;
                    case "SELECT COUNT(*)\nFROM \"UFForms\"\nWHERE (\"UFForms\".\"FolderKey\" NOT IN (SELECT \"UFFolders\".\"Key\" AS \"Key\"\nFROM \"UFFolders\"))\nAND (FolderKey Is Not Null)":
                        cmd.CommandText = "SELECT COUNT(*)\nFROM \"UFForms\"\nWHERE (\"UFForms\".\"FolderKey\" NOT IN (SELECT \"UFFolders\".\"Key\" AS \"Key\"\nFROM \"UFFolders\"))\nAND (\"FolderKey\" Is Not Null)";
                        break;
                    case "SELECT count(*) As Count,max(created) As LastSubmittedDate\nFROM \"UFRecords\"\nWHERE (Created >= @p0 AND Created <= @p1)\nAND (Form = @p2)":
                        cmd.CommandText = "SELECT COUNT(*) As \"Count\", MAX(\"Created\") As \"LastSubmittedDate\"\nFROM \"UFRecords\"\nWHERE (\"Created\" >= @p0 AND \"Created\" <= @p1)\nAND (\"Form\" = @p2)";
                        break;
                    case "SELECT \"Key\" AS \"Key\", \"FieldId\" AS \"FieldId\", \"Record\" AS \"Record\", \"Alias\" AS \"Alias\", \"DataType\" AS \"DataTypeAlias\" FROM \"UFRecordFields\" WHERE record = @p0":
                        cmd.CommandText = "SELECT \"Key\" AS \"Key\", \"FieldId\" AS \"FieldId\", \"Record\" AS \"Record\", \"Alias\" AS \"Alias\", \"DataType\" AS \"DataTypeAlias\" FROM \"UFRecordFields\" WHERE \"Record\" = @p0";
                        break;
                    case "SELECT COUNT(*) FROM (SELECT \"Id\" AS \"Id\", \"Form\" AS \"Form\", \"Created\" AS \"Created\", \"Updated\" AS \"Updated\", \"CurrentPage\" AS \"CurrentPage\", \"UmbracoPageId\" AS \"UmbracoPageId\", \"IP\" AS \"IP\", \"MemberKey\" AS \"MemberKey\", \"UniqueId\" AS \"UniqueId\", \"State\" AS \"StateAsString\", \"RecordData\" AS \"RecordData\", \"Culture\" AS \"Culture\", \"AdditionalData\" AS \"AdditionalData\" FROM \"UFRecords\"\nWHERE (Created >= @p0 AND Created <= @p1)\nAND (Form = @p2)\r\n) npoco_tbl":
                        cmd.CommandText = "SELECT COUNT(*) FROM (SELECT \"Id\" AS \"Id\", \"Form\" AS \"Form\", \"Created\" AS \"Created\", \"Updated\" AS \"Updated\", \"CurrentPage\" AS \"CurrentPage\", \"UmbracoPageId\" AS \"UmbracoPageId\", \"IP\" AS \"IP\", \"MemberKey\" AS \"MemberKey\", \"UniqueId\" AS \"UniqueId\", \"State\" AS \"StateAsString\", \"RecordData\" AS \"RecordData\", \"Culture\" AS \"Culture\", \"AdditionalData\" AS \"AdditionalData\" FROM \"UFRecords\"\r\nWHERE (\"Created\" >= @p0 AND \"Created\" <= @p1) AND (\"Form\" = @p2)) npoco_tbl";
                        break;
                    //case "SELECT COUNT(*) As \"Count\", MAX(\"Created\") As \"LastSubmittedDate\"\nFROM \"UFRecords\"\nWHERE (\"Created\" >= '@0' AND \"Created\" <= '@1')\nAND (\"Form\" = '@2')":

                    //    break;
                    //case "SELECT \"UFForms\".\"FolderKey\" AS \"FolderKey\", \"UFForms\".\"NodeId\" AS \"NodeId\", \"UFForms\".\"CreatedBy\" AS \"CreatedBy\", \"UFForms\".\"UpdatedBy\" AS \"UpdatedBy\", \"UFForms\".\"Id\" AS \"Id\", \"UFForms\".\"Key\" AS \"Key\", \"UFForms\".\"Name\" AS \"Name\", \"UFForms\".\"Definition\" AS \"Definition\", \"UFForms\".\"Created\" AS \"CreateDate\", \"UFForms\".\"Updated\" AS \"UpdateDate\"\nFROM \"UFForms\"\nWHERE ((\"UFForms\".\"Key\" = @p0))":
                    //    cmd.CommandText = "SELECT \"UFForms\".\"FolderKey\" AS \"FolderKey\", \"UFForms\".\"NodeId\" AS \"NodeId\", \"UFForms\".\"CreatedBy\" AS \"CreatedBy\", \"UFForms\".\"UpdatedBy\" AS \"UpdatedBy\", \"UFForms\".\"Id\" AS \"Id\", \"UFForms\".\"Key\" AS \"Key\", \"UFForms\".\"Name\" AS \"Name\", \"UFForms\".\"Definition\" AS \"Definition\", \"UFForms\".\"Created\" AS \"CreateDate\", \"UFForms\".\"Updated\" AS \"UpdateDate\"\nFROM \"UFForms\"\nWHERE ((\"UFForms\".\"Key\" = @p0))";
                    //    break;
                    default:
                        success = false;
                        break;
                }
            }
            else if (cmd.CommandText.StartsWith("INSERT "))
            {
                var tableName = string.Empty;
                switch (cmd.CommandText)
                {
                    case "INSERT INTO UFRecordDataString([Key], [Value]) VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataString\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        tableName = "UFRecordDataString";
                        break;
                    case "INSERT INTO UFRecordDataLongString(\"Key\", \"Value\") VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataLongString\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        tableName = "UFRecordDataLongString";
                        //cmd.CommandText += "ALTER SEQUENCE \"UFRecordDataLongString_Id_seq\" RESTART WITH SELECT MAX(\"Id\") + 1 FROM \"UFRecordDataLongString\";";
                        break;
                    case "INSERT INTO UFRecordDataInteger(\"Key\", \"Value\") VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataInteger\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        tableName = "UFRecordDataInteger";
                        //cmd.CommandText += "ALTER SEQUENCE \"UFRecordDataInteger_Id_seq\" RESTART WITH SELECT MAX(\"Id\") + 1 FROM \"UFRecordDataInteger\";";
                        break;
                    case "INSERT INTO UFRecordDataBit(\"Key\", \"Value\") VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataBit\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        tableName = "UFRecordDataBit";
                        //cmd.CommandText += "ALTER SEQUENCE \"UFRecordDataBit_Id_seq\" RESTART WITH SELECT MAX(\"Id\") + 1 FROM \"UFRecordDataBit\";";
                        break;
                    case "INSERT INTO UFRecordDataIDateTime(\"Key\", \"Value\") VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataIDateTime\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        tableName = "UFRecordDataIDateTime";
                        //cmd.CommandText += "ALTER SEQUENCE \"UFRecordDataIDateTime_Id_seq\" RESTART WITH SELECT MAX(\"Id\") + 1 FROM \"UFRecordDataIDateTime\";";
                        break;
                    case "INSERT INTO \"UFRecords\" (\"Form\",\"Created\",\"Updated\",\"CurrentPage\",\"UmbracoPageId\",\"IP\",\"MemberKey\",\"UniqueId\",\"State\",\"RecordData\",\"Culture\",\"AdditionalData\") VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11) returning \"id\" as id":
                        cmd.CommandText = "INSERT INTO \"UFRecords\" (\"Form\",\"Created\",\"Updated\",\"CurrentPage\",\"UmbracoPageId\",\"IP\",\"MemberKey\",\"UniqueId\",\"State\",\"RecordData\",\"Culture\",\"AdditionalData\") VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11) returning \"Id\" as id;";
                        tableName = "UFRecords";
                        //cmd.CommandText += "ALTER SEQUENCE \"UFRecords_Id_seq\" RESTART WITH SELECT MAX(\"Id\") + 1 FROM \"UFRecords\";";
                        break;
                    case "INSERT INTO \"UFForms\" (\"FolderKey\",\"NodeId\",\"CreatedBy\",\"UpdatedBy\",\"Key\",\"Name\",\"Definition\",\"Created\",\"Updated\") VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8) returning \"id\" as id":
                        cmd.CommandText = "INSERT INTO \"UFForms\" (\"FolderKey\",\"NodeId\",\"CreatedBy\",\"UpdatedBy\",\"Key\",\"Name\",\"Definition\",\"Created\",\"Updated\") VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8) returning \"Id\" as id;";
                        tableName = "UFForms";
                        //cmd.CommandText += "ALTER SEQUENCE \"UFRecords_Id_seq\" RESTART WITH SELECT MAX(\"Id\") + 1 FROM \"UFRecords\";";
                        break;
                    //case "INSERT INTO \"UFRecordFields\" (\"Key\",\"FieldId\",\"Record\",\"Alias\",\"DataType\") VALUES (@p0,@p1,@p2,@p3,@p4);\nINSERT INTO \"UFRecordFields\" (\"Key\",\"FieldId\",\"Record\",\"Alias\",\"DataType\") VALUES (@p5,@p6,@p7,@p8,@p9);\nINSERT INTO \"UFRecordFields\" (\"Key\",\"FieldId\",\"Record\",\"Alias\",\"DataType\") VALUES (@p10,@p11,@p12,@p13,@p14);\nINSERT INTO \"UFRecordFields\" (\"Key\",\"FieldId\",\"Record\",\"Alias\",\"DataType\") VALUES (@p15,@p16,@p17,@p18,@p19);":
                    //    cmd.CommandText = "INSERT INTO \"UFRecordFields\" (\"Key\",\"FieldId\",\"Record\",\"Alias\",\"DataType\") VALUES (@p0,@p1,@p2,@p3,@p4);\nINSERT INTO \"UFRecordFields\" (\"Key\",\"FieldId\",\"Record\",\"Alias\",\"DataType\") VALUES (@p5,@p6,@p7,@p8,@p9);\nINSERT INTO \"UFRecordFields\" (\"Key\",\"FieldId\",\"Record\",\"Alias\",\"DataType\") VALUES (@p10,@p11,@p12,@p13,@p14);\nINSERT INTO \"UFRecordFields\" (\"Key\",\"FieldId\",\"Record\",\"Alias\",\"DataType\") VALUES (@p15,@p16,@p17,@p18,@p19);";
                    //    break;
                    case "UFDataSource":
                        cmd.CommandText = "";
                        tableName = "UFDataSource";
                        break;
                    case "UFFolders":
                        cmd.CommandText = "";
                        tableName = "UFFolders";
                        break;
                    case "UFPrevalueSource":
                        cmd.CommandText = "";
                        tableName = "UFPrevalueSource";
                        break;
                    case "UFRecordAudit":
                        cmd.CommandText = "";
                        tableName = "UFRecordAudit";
                        break;
                    case "UFRecordWorkflowAudit":
                        cmd.CommandText = "";
                        tableName = "UFRecordWorkflowAudit";
                        break;
                    case "UFUserFormSecurity":
                        cmd.CommandText = "";
                        tableName = "UFUserFormSecurity";
                        break;
                    case "UFUserGroupFormSecurity":
                        cmd.CommandText = "";
                        tableName = "UFUserGroupFormSecurity";
                        break;
                    default:
                        success = false;
                        break;
                }

                if (success)
                {
                    // cmd.CommandText += $"\nALTER SEQUENCE \"{tableName}_Id_seq\" RESTART WITH (SELECT COALESCE(MAX(\"Id\"),1) + 1 FROM \"{tableName}\");";
                    return success;
                }

                var insertStart = "INSERT INTO \"";
                if (cmd.CommandText.StartsWith(insertStart))
                {
                    if (cmd.CommandText.Contains("UFDataSource"))
                    {

                    }
                    if (cmd.CommandText.Contains("UFFolders"))
                    {

                    }
                    if (cmd.CommandText.Contains("UFPrevalueSource"))
                    {

                    }
                    if (cmd.CommandText.Contains("UFRecordAudit"))
                    {

                    }
                    if (cmd.CommandText.Contains("UFRecordWorkflowAudit"))
                    {

                    }
                    if (cmd.CommandText.Contains("UFUserFormSecurity"))
                    {

                    }
                    if (cmd.CommandText.Contains("UFUserGroupFormSecurity"))
                    {

                    }

                    if (cmd.CommandText.InvariantContains(") returning \"id\" as id"))
                    {
                        if (UmbracoFormsDefinitions.UfTables.Any(table => cmd.CommandText[insertStart.Length..].StartsWith(table, StringComparison.OrdinalIgnoreCase)))
                        {
                            cmd.CommandText = cmd.CommandText.Replace(") returning \"id\" as id", ") returning \"Id\" as id;", StringComparison.OrdinalIgnoreCase);
                            success = true;
                        }
                    }
                    else if (cmd.CommandText.Contains(") returning "))
                    {
                        if (UmbracoFormsDefinitions.UfTables.Any(table => cmd.CommandText[insertStart.Length..].StartsWith(table, StringComparison.OrdinalIgnoreCase)))
                        {
                            cmd.CommandText = cmd.CommandText[..cmd.CommandText.IndexOf(" returning ", StringComparison.OrdinalIgnoreCase)];
                            success = true;
                        }
                    }
                }
            }
            else if (cmd.CommandText.StartsWith("UPDATE "))
            {
                switch (cmd.CommandText)
                {
                    case "UPDATE UFDataSource SET Created = Created AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFDataSource\" SET \"Created\" = \"Created\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFDataSource SET Updated = Updated AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFDataSource\" SET \"Updated\" = \"Updated\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFFolders SET Created = Created AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFFolders\" SET \"Created\" = \"Created\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFForms SET Updated = Updated AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFForms\" SET \"Updated\" = \"Updated\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFFolders SET Updated = Updated AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFFolders\" SET \"Updated\" = \"Updated\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFForms SET Created = Created AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFForms\" SET \"Created\" = \"Created\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFPrevalueSource SET Created = Created AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFPrevalueSource\" SET \"Created\" = \"Created\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFPrevalueSource SET Updated = Updated AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFPrevalueSource\" SET \"Updated\" = \"Updated\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFRecords SET Created = Created AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFRecords\" SET \"Created\" = \"Created\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFRecords SET Updated = Updated AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFRecords\" SET \"Updated\" = \"Updated\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFRecordAudit SET UpdatedOn = UpdatedOn AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFRecordAudit\" SET \"UpdatedOn\" = \"UpdatedOn\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFRecordDataDateTime SET Value = Value AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFRecordDataDateTime\" SET \"Value\" = \"Value\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFRecordWorkflowAudit SET ExecutedOn = ExecutedOn AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFRecordWorkflowAudit\" SET \"ExecutedOn\" = \"ExecutedOn\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFWorkflows SET Created = Created AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFWorkflows\" SET \"Created\" = \"Created\" {GetTimeZone()}";
                        break;
                    case "UPDATE UFWorkflows SET Updated = Updated AT TIME ZONE 'W. Europe Standard Time' AT TIME ZONE 'UTC'":
                        cmd.CommandText = $"UPDATE \"UFWorkflows\" SET \"Updated\" = \"Updated\" {GetTimeZone()}";
                        break;
                    case "UPDATE \"UFUserSecurity\" SET manageforms = @p0, managedatasources = @p1, manageprevaluesources = @p2, manageworkflows = @p3, viewEntries = @p4, editEntries = @p5, deleteEntries = @p6 WHERE \"user\" = @p7":
                        cmd.CommandText = "UPDATE \"UFUserSecurity\" SET \"ManageForms\" = @p0, \"ManageDataSources\" = @p1, \"ManagePreValueSources\" = @p2, \"ManageWorkflows\" = @p3, \"ViewEntries\" = @p4, \"EditEntries\" = @p5, \"DeleteEntries\" = @p6 WHERE \"User\" = @p7";
                        break;
                    default:
                        success = false;
                        break;
                }
            }

            if (cmd.CommandText.Contains('['))
            {
                cmd.CommandText = cmd.CommandText.Replace("[", "\"").Replace("]", "\"");
                success = true;
            }

            // success = success && ConvertParameters(cmd);

            return success;
        }

        private bool ConvertParameters(DbCommand cmd)
        {
            foreach (DbParameter parameter in cmd.Parameters)
            {
                if (parameter.DbType is DbType.Guid)
                {
                    // parameter.Value = parameter.Value?.ToString();
                }
            }

            return true;
        }
    }
}
