using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Our.Umbraco.PostgreSql.Services;
using System.Data;
using System.Data.Common;
using System.Text;
using Umbraco.Cms.Core.Models;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql.Umbraco.Forms
{
    /// <summary>
    /// Provides functionality to correct Umbraco.Forms SQL statements and validation timestamp updates in PostgreSQL database commands.
    /// </summary>
    public class PostgreSqlFixUmbracoFormsService : PostgreSqlFixServiceBase
    {
        private string[] UfTables
            => [
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
                "UFWorkflows"
                ];

        private bool ContainsUfTableName(string sqlStatement, string? tableName = null)
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
        public override bool FixCommanText(DbCommand cmd) => FixUmbracoFormsIssues(cmd);

        private bool FixUmbracoFormsIssues(DbCommand cmd)
        {
            var success = true;

            if (!IsUfCommand(cmd))
            {
                return success;
            }

            if (cmd.CommandText.Contains('['))
            {
                cmd.CommandText = cmd.CommandText.Replace("[", "\"").Replace("]", "\"");
            }

            if (cmd.CommandText.Contains("@0"))
            {
                for (int i = 0; i < cmd.Parameters.Count; i++)
                {
                    cmd.CommandText = cmd.CommandText.Replace($"@{i}", $"@p{i}");
                }
            }

            if (cmd.CommandText.StartsWith("SELECT "))
            {
                //if (cmd.CommandText.StartsWith("SELECT \"Form\", COUNT(*) as \"Total\"\nFROM UFRecords\nWHERE (\"Created\" >= @p0 AND \"Created\" <= @p1)\nAND (\"Form\" IN ("))
                if   (cmd.CommandText.StartsWith("SELECT \"Form\", COUNT(*) as \"Total\"\nFROM UFRecords\nWHERE (\"Created\" >= @p0 AND \"Created\" <= @p1)\nAND (\"Form\" IN ("))
                {
                    cmd.CommandText = cmd.CommandText.Replace("FROM UFRecords", "FROM \"UFRecords\"");
                    return success;
                }
                else if (cmd.CommandText.Contains("WHERE RecordUniqueId IN ("))
                {
                    cmd.CommandText = cmd.CommandText.Replace("WHERE RecordUniqueId IN (", "WHERE \"RecordUniqueId\" IN (");
                    return success;
                }
                else
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
                        //case "SELECT \"Form\", COUNT(*) as \"Total\"\nFROM UFRecords\nWHERE (\"Created\" >= @p0 AND \"Created\" <= @p1)\nAND (\"Form\" IN (@p2,@p3))\nGROUP BY \"Form\"":
                        //    cmd.CommandText = "SELECT \"Form\", COUNT(*) as \"Total\" FROM \"UFRecords\" WHERE (\"Created\" >= @p0 AND \"Created\" <= @p1) AND (\"Form\" IN (@p2,@p3)) GROUP BY \"Form\"";
                        //    break;
                        //case "SELECT \"Form\", COUNT(*) as \"Total\"\nFROM UFRecords\nWHERE (\"Created\" >= @p0 AND \"Created\" <= @p1)\nAND (\"Form\" IN (@p2,@p3,@p4))\nGROUP BY \"Form\"":
                        //    cmd.CommandText = "SELECT \"Form\", COUNT(*) as \"Total\" FROM \"UFRecords\" WHERE (\"Created\" >= @p0 AND \"Created\" <= @p1) AND (\"Form\" IN (@p2,@p3,@p4)) GROUP BY \"Form\"";
                        //    break;
                        case "SELECT \"Key\" AS \"Key\", \"FieldId\" AS \"FieldId\", \"Record\" AS \"Record\", \"Alias\" AS \"Alias\", \"DataType\" AS \"DataTypeAlias\" FROM \"UFRecordFields\" WHERE record = @p0":
                            cmd.CommandText = "SELECT \"Key\" AS \"Key\", \"FieldId\" AS \"FieldId\", \"Record\" AS \"Record\", \"Alias\" AS \"Alias\", \"DataType\" AS \"DataTypeAlias\" FROM \"UFRecordFields\" WHERE \"Record\" = @p0";
                            break;
                        case "SELECT COUNT(*) FROM (SELECT \"Id\" AS \"Id\", \"Form\" AS \"Form\", \"Created\" AS \"Created\", \"Updated\" AS \"Updated\", \"CurrentPage\" AS \"CurrentPage\", \"UmbracoPageId\" AS \"UmbracoPageId\", \"IP\" AS \"IP\", \"MemberKey\" AS \"MemberKey\", \"UniqueId\" AS \"UniqueId\", \"State\" AS \"StateAsString\", \"RecordData\" AS \"RecordData\", \"Culture\" AS \"Culture\", \"AdditionalData\" AS \"AdditionalData\" FROM \"UFRecords\"\nWHERE (Created >= @p0 AND Created <= @p1)\nAND (Form = @p2)\n) npoco_tbl":
                            cmd.CommandText = "SELECT COUNT(*) FROM \"UFRecords\" WHERE (\"Created\" >= @p0 AND \"Created\" <= @p1) AND \"Form\" = @p2;";
                            break;
                        case "SELECT \"Id\" AS \"Id\", \"Form\" AS \"Form\", \"Created\" AS \"Created\", \"Updated\" AS \"Updated\", \"CurrentPage\" AS \"CurrentPage\", \"UmbracoPageId\" AS \"UmbracoPageId\", \"IP\" AS \"IP\", \"MemberKey\" AS \"MemberKey\", \"UniqueId\" AS \"UniqueId\", \"State\" AS \"StateAsString\", \"RecordData\" AS \"RecordData\", \"Culture\" AS \"Culture\", \"AdditionalData\" AS \"AdditionalData\" FROM \"UFRecords\"\nWHERE (Created >= @p0 AND Created <= @p1)\nAND (Form = @p2)\nORDER BY created DESC\nLIMIT @p3 OFFSET @p4":
                            cmd.CommandText = "SELECT \"Id\" AS \"Id\", \"Form\" AS \"Form\", \"Created\" AS \"Created\", \"Updated\" AS \"Updated\", \"CurrentPage\" AS \"CurrentPage\", \"UmbracoPageId\" AS \"UmbracoPageId\", \"IP\" AS \"IP\", \"MemberKey\" AS \"MemberKey\", \"UniqueId\" AS \"UniqueId\", \"State\" AS \"StateAsString\", \"RecordData\" AS \"RecordData\", \"Culture\" AS \"Culture\", \"AdditionalData\" AS \"AdditionalData\" FROM \"UFRecords\" WHERE (\"Created\" >= @p0 AND \"Created\" <= @p1) AND (\"Form\" = @p2) ORDER BY \"Created\" DESC LIMIT @p3 OFFSET @p4";
                            break;
                        //case "SELECT \"Id\" AS \"Id\", \"RecordUniqueId\" AS \"RecordUniqueId\", \"WorkflowKey\" AS \"WorkflowKey\", \"WorkflowName\" AS \"WorkflowName\", \"WorkflowTypeId\" AS \"WorkflowTypeId\", \"WorkflowTypeName\" AS \"WorkflowTypeName\", \"ExecutedOn\" AS \"ExecutedOn\", \"ExecutionStage\" AS \"ExecutionStage\", \"ExecutionStatus\" AS \"ExecutionStatus\" FROM \"UFRecordWorkflowAudit\" WHERE RecordUniqueId IN (@p0)":
                        //    cmd.CommandText = "SELECT \"Id\" AS \"Id\", \"RecordUniqueId\" AS \"RecordUniqueId\", \"WorkflowKey\" AS \"WorkflowKey\", \"WorkflowName\" AS \"WorkflowName\", \"WorkflowTypeId\" AS \"WorkflowTypeId\", \"WorkflowTypeName\" AS \"WorkflowTypeName\", \"ExecutedOn\" AS \"ExecutedOn\", \"ExecutionStage\" AS \"ExecutionStage\", \"ExecutionStatus\" AS \"ExecutionStatus\" FROM \"UFRecordWorkflowAudit\" WHERE \"RecordUniqueId\" IN (@p0)";
                        //    break;
                        //case "SELECT \"Id\" AS \"Id\", \"RecordUniqueId\" AS \"RecordUniqueId\", \"WorkflowKey\" AS \"WorkflowKey\", \"WorkflowName\" AS \"WorkflowName\", \"WorkflowTypeId\" AS \"WorkflowTypeId\", \"WorkflowTypeName\" AS \"WorkflowTypeName\", \"ExecutedOn\" AS \"ExecutedOn\", \"ExecutionStage\" AS \"ExecutionStage\", \"ExecutionStatus\" AS \"ExecutionStatus\" FROM \"UFRecordWorkflowAudit\" WHERE RecordUniqueId IN (@p0, @p1)":
                        //    cmd.CommandText = "SELECT \"Id\" AS \"Id\", \"RecordUniqueId\" AS \"RecordUniqueId\", \"WorkflowKey\" AS \"WorkflowKey\", \"WorkflowName\" AS \"WorkflowName\", \"WorkflowTypeId\" AS \"WorkflowTypeId\", \"WorkflowTypeName\" AS \"WorkflowTypeName\", \"ExecutedOn\" AS \"ExecutedOn\", \"ExecutionStage\" AS \"ExecutionStage\", \"ExecutionStatus\" AS \"ExecutionStatus\" FROM \"UFRecordWorkflowAudit\" WHERE \"RecordUniqueId\" IN (@p0, @p1)";
                        //    break;                       
                        //case "SELECT \"Id\" AS \"Id\", \"RecordUniqueId\" AS \"RecordUniqueId\", \"WorkflowKey\" AS \"WorkflowKey\", \"WorkflowName\" AS \"WorkflowName\", \"WorkflowTypeId\" AS \"WorkflowTypeId\", \"WorkflowTypeName\" AS \"WorkflowTypeName\", \"ExecutedOn\" AS \"ExecutedOn\", \"ExecutionStage\" AS \"ExecutionStage\", \"ExecutionStatus\" AS \"ExecutionStatus\" FROM \"UFRecordWorkflowAudit\" WHERE RecordUniqueId IN (@p0, @p1, @p2)":
                        //    cmd.CommandText = "SELECT \"Id\" AS \"Id\", \"RecordUniqueId\" AS \"RecordUniqueId\", \"WorkflowKey\" AS \"WorkflowKey\", \"WorkflowName\" AS \"WorkflowName\", \"WorkflowTypeId\" AS \"WorkflowTypeId\", \"WorkflowTypeName\" AS \"WorkflowTypeName\", \"ExecutedOn\" AS \"ExecutedOn\", \"ExecutionStage\" AS \"ExecutionStage\", \"ExecutionStatus\" AS \"ExecutionStatus\" FROM \"UFRecordWorkflowAudit\" WHERE \"RecordUniqueId\" IN (@p0, @p1, @p2)";
                        //    break;
                        case "SELECT COUNT(*)\nFROM sys.indexes\nWHERE (object_id = OBJECT_ID(@p0) AND name = @p1)":
                            cmd.CommandText = "SELECT COUNT(*) FROM pg_class t INNER JOIN pg_index ix ON t.oid = ix.indrelid INNER JOIN pg_class i ON i.oid = ix.indexrelid INNER JOIN pg_namespace n ON n.oid = t.relnamespace LEFT JOIN pg_attribute a ON a.attrelid = t.oid AND a.attnum = ANY(ix.indkey) WHERE (t.relname = @p0 AND i.relname = @p1)";
                            break;
                        case "SELECT MIN(\"Created\")\nFROM UFRecords":
                            cmd.CommandText = "SELECT MIN(\"Created\") FROM \"UFRecords\"";
                            break;
                        case "SELECT MIN(\"Date\")\nFROM UFAnalyticsProcessedDates":
                            cmd.CommandText = "SELECT MIN(\"Date\") FROM \"UFAnalyticsProcessedDates\"";
                            break;
                        case "SELECT *\nFROM UFAnalyticsProcessedDates\nWHERE (\"Date\" >= @p0 AND \"Date\" <= @p1)":
                            cmd.CommandText = "SELECT * FROM \"UFAnalyticsProcessedDates\" WHERE (\"Date\" >= @p0 AND \"Date\" <= @p1)";
                            break;
                        case "SELECT COUNT(*)\nFROM UFAnalyticsProcessedDates\nWHERE (\"Date\" >= @p0 AND \"Date\" <= @p1)":
                            cmd.CommandText = "SELECT COUNT(*) FROM \"UFAnalyticsProcessedDates\" WHERE (\"Date\" >= @p0 AND \"Date\" <= @p1)";
                            break;
                        case "SELECT \"Id\" AS \"Id\", \"Form\" AS \"Form\", \"Created\" AS \"Created\", \"Updated\" AS \"Updated\", \"CurrentPage\" AS \"CurrentPage\", \"UmbracoPageId\" AS \"UmbracoPageId\", \"IP\" AS \"IP\", \"MemberKey\" AS \"MemberKey\", \"UniqueId\" AS \"UniqueId\", \"State\" AS \"StateAsString\", \"RecordData\" AS \"RecordData\", \"Culture\" AS \"Culture\", \"AdditionalData\" AS \"AdditionalData\" FROM \"UFRecords\"\nWHERE (Created >= @p0 AND Created <= @p1)\nAND (Form = @p2)\nAND (UniqueId in (@p3))":
                            cmd.CommandText = "SELECT \"Id\" AS \"Id\", \"Form\" AS \"Form\", \"Created\" AS \"Created\", \"Updated\" AS \"Updated\", \"CurrentPage\" AS \"CurrentPage\", \"UmbracoPageId\" AS \"UmbracoPageId\", \"IP\" AS \"IP\", \"MemberKey\" AS \"MemberKey\", \"UniqueId\" AS \"UniqueId\", \"State\" AS \"StateAsString\", \"RecordData\" AS \"RecordData\", \"Culture\" AS \"Culture\", \"AdditionalData\" AS \"AdditionalData\" FROM \"UFRecords\" WHERE (\"Created\" >= @p0 AND \"Created\" <= @p1) AND (\"Form\" = @p2) AND (\"UniqueId\" IN (@p3))";
                            break;
                        case "SELECT \"Form\", CAST(\"Created\" AS DATE) as \"Day\", DATEPART(HOUR, \"Created\") as \"Hour\", \"UmbracoPageId\", COUNT(*) as \"Total\"\nFROM UFRecords\nWHERE (\"Created\" >= @p0 AND \"Created\" < @p1)\nGROUP BY \"Form\", CAST(\"Created\" AS DATE), DATEPART(HOUR, \"Created\"), \"UmbracoPageId\"":
                            cmd.CommandText = "SELECT \"Form\", CAST(\"Created\" AS DATE) as \"Day\", EXTRACT(HOUR FROM \"Created\") as \"Hour\", \"UmbracoPageId\", COUNT(*) as \"Total\" FROM \"UFRecords\" WHERE (\"Created\" >= @p0 AND \"Created\" < @p1) GROUP BY \"Form\", CAST(\"Created\" AS DATE), EXTRACT(HOUR FROM \"Created\"), \"UmbracoPageId\"";
                            break;
                        default:
                            success = false;
                            break;
                    }
                }
            }
            else if (cmd.CommandText.StartsWith("INSERT "))
            {
                var tableName = string.Empty;
                switch (cmd.CommandText)
                {
                    case "INSERT INTO UFRecordDataString(\"Key\", \"Value\") VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataString\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        tableName = "UFRecordDataString";
                        break;
                    case "INSERT INTO UFRecordDataLongString(\"Key\", \"Value\") VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataLongString\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        tableName = "UFRecordDataLongString";
                        break;
                    case "INSERT INTO UFRecordDataInteger(\"Key\", \"Value\") VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataInteger\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        tableName = "UFRecordDataInteger";
                        break;
                    case "INSERT INTO UFRecordDataBit(\"Key\", \"Value\") VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataBit\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        tableName = "UFRecordDataBit";
                        break;
                    case "INSERT INTO UFRecordDataIDateTime(\"Key\", \"Value\") VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataIDateTime\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        tableName = "UFRecordDataIDateTime";
                        break;
                    case "INSERT INTO \"UFRecords\" (\"Form\",\"Created\",\"Updated\",\"CurrentPage\",\"UmbracoPageId\",\"IP\",\"MemberKey\",\"UniqueId\",\"State\",\"RecordData\",\"Culture\",\"AdditionalData\") VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11) returning \"id\" as id":
                        cmd.CommandText = "INSERT INTO \"UFRecords\" (\"Form\",\"Created\",\"Updated\",\"CurrentPage\",\"UmbracoPageId\",\"IP\",\"MemberKey\",\"UniqueId\",\"State\",\"RecordData\",\"Culture\",\"AdditionalData\") VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11) returning \"Id\" as id;";
                        tableName = "UFRecords";
                        break;
                    case "INSERT INTO \"UFForms\" (\"FolderKey\",\"NodeId\",\"CreatedBy\",\"UpdatedBy\",\"Key\",\"Name\",\"Definition\",\"Created\",\"Updated\") VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8) returning \"id\" as id":
                        cmd.CommandText = "INSERT INTO \"UFForms\" (\"FolderKey\",\"NodeId\",\"CreatedBy\",\"UpdatedBy\",\"Key\",\"Name\",\"Definition\",\"Created\",\"Updated\") VALUES (@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8) returning \"Id\" as id;";
                        tableName = "UFForms";
                        break;

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
                    if (cmd.CommandText.Contains(") returning "))
                    {
                        if (cmd.CommandText.InvariantContains(") returning \"id\" as id"))
                        {
                            cmd.CommandText = cmd.CommandText.Replace(") returning \"id\" as id", ") returning \"Id\" as id;", StringComparison.OrdinalIgnoreCase);
                            success = true;
                        }
                        else
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
                        cmd.CommandText = "UPDATE \"UFUserSecurity\" SET \"ManageForms\" = @p0, \"ManageDataSources\" = @p1, \"ManagePreValueSources\" = @p2, \"ManageWorkflows\" = @p3, \"ViewEntries\" = @p4, \"EditEntries\" = @p5, \"DeleteEntries\" = @p6 WHERE \"User\" = '@p7'";
                        break;
                    case "UPDATE \"UFUserFormSecurity\" SET HasAccess = @p0, SecurityType = @p1, AllowInEditor = @p2 WHERE \"user\" = @p3 AND form = @p4":
                        cmd.CommandText = "UPDATE \"UFUserFormSecurity\" SET \"HasAccess\" = @p0, \"SecurityType\" = @p1, \"AllowInEditor\" = @p2 WHERE \"User\" = '@p3' AND \"Form\" = @p4";
                        break;
                    default:
                        success = false;
                        break;
                }
            }
            else if (cmd.CommandText.StartsWith("DELETE "))
            {
                switch (cmd.CommandText)
                {
                    case "DELETE FROM UFRecordDataString WHERE record = @p0":
                        cmd.CommandText = "DELETE FROM \"UFRecordDataString\" WHERE \"Record\" = @p0";
                        break;
                    case "DELETE FROM UFRecordDataLongString WHERE record = @p0":
                        cmd.CommandText = "DELETE FROM \"UFRecordDataLongString\" WHERE \"Record\" = @p0";
                        break;
                    case "DELETE FROM UFRecordDataInteger WHERE record = @p0":
                        cmd.CommandText = "DELETE FROM \"UFRecordDataInteger\" WHERE \"Record\" = @p0";
                        break;
                    case "DELETE FROM UFRecordDataBit WHERE record = @p0":
                        cmd.CommandText = "DELETE FROM \"UFRecordDataBit\" WHERE \"Record\" = @p0";
                        break;
                    case "DELETE FROM UFRecordDataIDateTime WHERE record = @p0":
                        cmd.CommandText = "DELETE FROM \"UFRecordDataIDateTime\" WHERE \"Record\" = @p0";
                        break;
                    case "DELETE FROM UFRecordFields WHERE UFRecordFields.Record in (@p0)":
                        cmd.CommandText = "DELETE FROM \"UFRecordFields\" WHERE \"Record\" IN (@p0)";
                        break;
                    case "DELETE FROM \"UFUserStartFolders\" WHERE UserId = @p0":
                        cmd.CommandText = "DELETE FROM \"UFUserStartFolders\" WHERE \"UserId\" = @p0";
                        break;
                    case "DELETE FROM UFPrevalueSource WHERE \"key\" = @p0":
                        cmd.CommandText = "DELETE FROM \"UFPrevalueSource\" WHERE \"Key\" = @p0";
                        break;
                    case "DELETE FROM UFDataSource WHERE \"key\" = @p0":
                        cmd.CommandText = "DELETE FROM \"UFDataSource\" WHERE \"Key\" = @p0";
                        break;
                    default:
                        success = false;
                        break;
                }
            }

            return success;
        }

        private bool IsUfCommand(DbCommand cmd)
        {
            return
                string.IsNullOrEmpty(cmd.CommandText)
                || cmd.CommandText.Contains(" UF")
                || cmd.CommandText.Contains(" \"UF")
                || cmd.CommandText.Contains("sys.indexes");
        }
        public override Func<object, object>? GetParameterConverter(DbCommand cmd, Type sourceType)
        {
            if (!IsUfCommand(cmd))
            {
                return null;
            }

            return value =>
            {
                if (value is string str && Guid.TryParse(str, out Guid guidValue))
                {
                    return guidValue;
                }

                if (value is int i
                    && (i == 0 || i == 1)
                    && bool.TryParse(i.ToString(), out bool boolValue))
                {
                    return boolValue;
                }

                return value;
            };
        }
    }
}
