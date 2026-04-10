using System.Data;
using System.Data.Common;
using System.Text;
using J2N.Text;
using Microsoft.Extensions.Logging;
using Our.Umbraco.PostgreSql.Services;
using Umbraco.Extensions;

namespace Our.Umbraco.PostgreSql.Umbraco.Forms
{
    /// <summary>
    /// Provides functionality to correct Umbraco.Forms SQL statements and validation timestamp updates in PostgreSQL database commands.
    /// </summary>
    public class PostgreSqlFixUmbracoFormsService : PostgreSqlFixServiceBase
    {
        public override bool FixCommanText(DbCommand cmd) => FixUmbracoLicenseIssues(cmd);

        private bool FixUmbracoLicenseIssues(DbCommand cmd)
        {
            var success = true;

            if (!(cmd.CommandText.Contains(" UF") || cmd.CommandText.Contains(" \"UF") || cmd.CommandText.Contains("sys.indexes")))
            {
                return success;
            }

            string[] ufTables = [
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
                "UFWorkflows",
                ];

            var oldCommandText = cmd.CommandText;
            var cleanedCommandText = oldCommandText.Replace("[user]", "\"user\"");

            for (int i = 0; i < cmd.Parameters.Count; i++)
            {
                cleanedCommandText = cleanedCommandText.Replace($"@{i}", $"@p{i}");
            }

            if (cleanedCommandText.StartsWith("SELECT "))
            {
                if (cleanedCommandText.StartsWith("SELECT \"Form\", COUNT(*) AS \"Total\"\nFROM \"UFRecords\"\nWHERE (\"Created\" >= @p0 AND \"Created\" <= @p1)\nAND (\"Form\" IN (")
                    || cleanedCommandText.StartsWith("SELECT [Form], COUNT(*) AS [Total]\nFROM \"UFRecords\"\nWHERE ([Created] >= @p0 AND [Created] <= @p1)\nAND ([Form] IN ("))
                {
                    var paramList = new StringBuilder("SELECT \"Form\", COUNT(*) AS \"Total\" FROM \"UFRecords\" WHERE (\"Created\" >= @p0 AND \"Created\" <= @p1) AND (\"Form\"::text IN (");

                    for (int i = 2; i < cmd.Parameters.Count; i++)
                    {
                        if (i > 2)
                        {
                            paramList.Append($", ");
                        }

                        paramList.Append($"@p{i}");
                    }

                    paramList.Append(")) GROUP BY \"Form\"");

                    cmd.CommandText = paramList.ToString();
                }
                else
                {
                    switch (cleanedCommandText)
                    {
                        case "SELECT COUNT(*)\nFROM \"UFFolders\"\nWHERE (\"UFFolders\".\"ParentKey\" NOT IN (SELECT \"UFFolders\".\"Key\" AS \"Key\"\nFROM \"UFFolders\"))\nAND (ParentKey Is Not Null)":
                            cmd.CommandText = "SELECT COUNT(*) FROM \"UFFolders\" WHERE (\"UFFolders\".\"ParentKey\" NOT IN (SELECT \"UFFolders\".\"Key\" AS \"Key\" FROM \"UFFolders\")) AND (\"ParentKey\" Is Not Null)";
                            break;
                        case "SELECT COUNT(*)\nFROM \"UFForms\"\nWHERE (\"UFForms\".\"FolderKey\" NOT IN (SELECT \"UFFolders\".\"Key\" AS \"Key\"\nFROM \"UFFolders\"))\nAND (FolderKey Is Not Null)":
                            cmd.CommandText = "SELECT COUNT(*) FROM \"UFForms\" WHERE (\"UFForms\".\"FolderKey\" NOT IN (SELECT \"UFFolders\".\"Key\" AS \"Key\" FROM \"UFFolders\")) AND (\"FolderKey\" Is Not Null)";
                            break;
                        case "SELECT count(*) As Count,max(created) As LastSubmittedDate\nFROM \"UFRecords\"\nWHERE (Created >= @p0 AND Created <= @p1)\nAND (Form = @p2)":
                            cmd.CommandText = "SELECT COUNT(*) As \"Count\", MAX(\"Created\") As \"LastSubmittedDate\" FROM \"UFRecords\" WHERE (\"Created\" >= @p0 AND \"Created\" <= @p1) AND (\"Form\" = @p2)";
                            break;
                        case "SELECT COUNT(*)\nFROM sys.indexes\nWHERE (object_id = OBJECT_ID(@p0) AND name = @p1)":
                            cmd.CommandText = "SELECT COUNT(*) FROM pg_indexes WHERE (tablename = @p0 AND indexname = @p1)";
                            break;
                        case "SELECT COUNT(*) FROM (SELECT \"Id\" AS \"Id\", \"Form\" AS \"Form\", \"Created\" AS \"Created\", \"Updated\" AS \"Updated\", \"CurrentPage\" AS \"CurrentPage\", \"UmbracoPageId\" AS \"UmbracoPageId\", \"IP\" AS \"IP\", \"MemberKey\" AS \"MemberKey\", \"UniqueId\" AS \"UniqueId\", \"State\" AS \"StateAsString\", \"RecordData\" AS \"RecordData\", \"Culture\" AS \"Culture\", \"AdditionalData\" AS \"AdditionalData\" FROM \"UFRecords\"\nWHERE (Created >= @p0 AND Created <= @p1)\nAND (Form = @p2)\n) npoco_tbl":
                            cmd.CommandText = "SELECT COUNT(*) FROM (SELECT \"Id\" AS \"Id\", \"Form\" AS \"Form\", \"Created\" AS \"Created\", \"Updated\" AS \"Updated\", \"CurrentPage\" AS \"CurrentPage\", \"UmbracoPageId\" AS \"UmbracoPageId\", \"IP\" AS \"IP\", \"MemberKey\" AS \"MemberKey\", \"UniqueId\" AS \"UniqueId\", \"State\" AS \"StateAsString\", \"RecordData\" AS \"RecordData\", \"Culture\" AS \"Culture\", \"AdditionalData\" AS \"AdditionalData\" FROM \"UFRecords\"\nWHERE (\"Created\" >= @p0 AND \"Created\" <= @p1)\nAND (\"Form\" = @p2)\n)";
                            cmd.Parameters["@p0"].Value = EnsureUtc(cmd.Parameters["@p0"].Value);
                            cmd.Parameters["@p1"].Value = EnsureUtc(cmd.Parameters["@p1"].Value);
                            break;
                        case "SELECT \"UFForms\".\"FolderKey\" AS \"FolderKey\", \"UFForms\".\"NodeId\" AS \"NodeId\", \"UFForms\".\"CreatedBy\" AS \"CreatedBy\", \"UFForms\".\"UpdatedBy\" AS \"UpdatedBy\", \"UFForms\".\"Id\" AS \"Id\", \"UFForms\".\"Key\" AS \"Key\", \"UFForms\".\"Name\" AS \"Name\", \"UFForms\".\"Definition\" AS \"Definition\", \"UFForms\".\"Created\" AS \"CreateDate\", \"UFForms\".\"Updated\" AS \"UpdateDate\"\nFROM \"UFForms\"\nWHERE ((\"UFForms\".\"Key\" = @p0))":
                            cmd.CommandText = "SELECT \"UFForms\".\"FolderKey\" AS \"FolderKey\", \"UFForms\".\"NodeId\" AS \"NodeId\", \"UFForms\".\"CreatedBy\" AS \"CreatedBy\", \"UFForms\".\"UpdatedBy\" AS \"UpdatedBy\", \"UFForms\".\"Id\" AS \"Id\", \"UFForms\".\"Key\" AS \"Key\", \"UFForms\".\"Name\" AS \"Name\", \"UFForms\".\"Definition\" AS \"Definition\", \"UFForms\".\"Created\" AS \"CreateDate\", \"UFForms\".\"Updated\" AS \"UpdateDate\"\nFROM \"UFForms\"\nWHERE ((\"UFForms\".\"Key\" = @p0))";
                            break;
                        case "SELECT MIN([Created])\nFROM UFRecords":
                            cmd.CommandText = "SELECT MIN(\"Created\") FROM \"UFRecords\"";
                            break;
                        case "SELECT MIN([Date])\nFROM UFAnalyticsProcessedDates":
                            cmd.CommandText = "SELECT MIN(\"Date\") FROM \"UFAnalyticsProcessedDates\"";
                            break;
                        case "SELECT *\nFROM UFAnalyticsProcessedDates\nWHERE (\"Date\" >= @p0 AND \"Date\" <= @p1)":
                            cmd.CommandText = "SELECT * FROM \"UFAnalyticsProcessedDates\" WHERE (\"Date\" >= @p0 AND \"Date\" <= @p1)";
                            cmd.Parameters["@p0"].Value = EnsureUtc(cmd.Parameters["@p0"].Value);
                            cmd.Parameters["@p1"].Value = EnsureUtc(cmd.Parameters["@p1"].Value);
                            break;
                        case "SELECT \"Form\", CAST(\"Created\" AS DATE) as \"Day\", DATEPART(HOUR, \"Created\") as \"Hour\", \"UmbracoPageId\", COUNT(*) as \"Total\"\nFROM UFRecords\nWHERE (\"Created\" >= @p0 AND \"Created\" < @p1)\nGROUP BY \"Form\", CAST(\"Created\" AS DATE), DATEPART(HOUR, \"Created\"), \"UmbracoPageId\"":
                            cmd.CommandText = "SELECT \"Form\", CAST(\"Created\" AS DATE) as \"Day\", DATE_PART('HOUR', \"Created\") as \"Hour\", \"UmbracoPageId\", COUNT(*) as \"Total\" FROM \"UFRecords\" WHERE (\"Created\" >= @p0 AND \"Created\" < @p1) GROUP BY \"Form\", CAST(\"Created\" AS DATE), DATE_PART('HOUR', \"Created\"), \"UmbracoPageId\"";
                            cmd.Parameters["@p0"].Value = EnsureUtc(cmd.Parameters["@p0"].Value);
                            cmd.Parameters["@p1"].Value = EnsureUtc(cmd.Parameters["@p1"].Value);
                            break;
                        case "SELECT \"UmbracoPageId\", COUNT(*) as \"Total\"\nFROM UFRecords\nWHERE (\"Form\" = @p0)\nAND (\"Created\" >= @p1 AND \"Created\" <= @p2)\nAND (\"UmbracoPageId\" IS NOT NULL AND \"UmbracoPageId\" > 0)\nGROUP BY \"UmbracoPageId\"":
                            cmd.CommandText = "SELECT \"UmbracoPageId\", COUNT(*) as \"Total\" FROM \"UFRecords\" WHERE (\"Form\" = @p0) AND (\"Created\" >= @p1 AND \"Created\" <= @p2) AND (\"UmbracoPageId\" IS NOT NULL AND \"UmbracoPageId\" > 0) GROUP BY \"UmbracoPageId\"";
                            cmd.Parameters["@p1"].Value = EnsureUtc(cmd.Parameters["@p1"].Value);
                            cmd.Parameters["@p2"].Value = EnsureUtc(cmd.Parameters["@p2"].Value);
                            break;
                        case "SELECT r.\"Form\", CAST(wfa.\"ExecutedOn\" AS DATE) as \"Day\", wfa.\"WorkflowKey\", COUNT(*) as \"Triggered\", SUM(CASE WHEN wfa.\"ExecutionStatus\" = 0 THEN 1 ELSE 0 END) as \"Failures\"\nFROM UFRecordWorkflowAudit wfa\nINNER JOIN UFRecords r\nON wfa.\"RecordUniqueId\" = r.\"UniqueId\"\nWHERE (wfa.\"ExecutedOn\" >= @p0 AND wfa.\"ExecutedOn\" < @p1)\nGROUP BY r.\"Form\", CAST(wfa.\"ExecutedOn\" AS DATE), wfa.\"WorkflowKey\"":
                            cmd.CommandText = "SELECT r.\"Form\", CAST(wfa.\"ExecutedOn\" AS DATE) as \"Day\", wfa.\"WorkflowKey\", COUNT(*) as \"Triggered\", SUM(CASE WHEN wfa.\"ExecutionStatus\" = 0 THEN 1 ELSE 0 END) as \"Failures\" FROM \"UFRecordWorkflowAudit\" wfa INNER JOIN \"UFRecords\" r ON wfa.\"RecordUniqueId\" = r.\"UniqueId\" WHERE (wfa.\"ExecutedOn\" >= @p0 AND wfa.\"ExecutedOn\" < @p1) GROUP BY r.\"Form\", CAST(wfa.\"ExecutedOn\" AS DATE), wfa.\"WorkflowKey\"";
                            cmd.Parameters["@p0"].Value = EnsureUtc(cmd.Parameters["@p0"].Value);
                            cmd.Parameters["@p1"].Value = EnsureUtc(cmd.Parameters["@p1"].Value);
                            break;
                        default:
                            success = false;
                            break;
                    }
                }
            }
            else if (cleanedCommandText.StartsWith("INSERT "))
            {
                switch (cleanedCommandText)
                {
                    case "INSERT INTO UFRecordDataBit([Key], [Value]) VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataBit\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        break;
                    case "INSERT INTO UFRecordDataDateTime([Key], [Value]) VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataDateTime\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        break;
                    case "INSERT INTO UFRecordDataInteger([Key], [Value]) VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataInteger\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        break;
                    case "INSERT INTO UFRecordDataString([Key], [Value]) VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataString\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        break;
                    case "INSERT INTO UFRecordDataLongString([Key], [Value]) VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataLongString\" (\"Key\", \"Value\") VALUES (@p0, @p1)";
                        break;
                    default:
                        success = false;
                        break;
                }

                var insertStart = "INSERT INTO \"";
                if (cmd.CommandText.StartsWith(insertStart))
                {
                    if (cmd.CommandText.Contains(") returning "))
                    {
                        if (ufTables.Any(table => cmd.CommandText[insertStart.Length..].StartsWith(table, StringComparison.OrdinalIgnoreCase)))
                        {
                            cmd.CommandText = cmd.CommandText[..cmd.CommandText.IndexOf(" returning ", StringComparison.OrdinalIgnoreCase)];
                            success = true;
                        }
                    }
                }
            }
            else if (cleanedCommandText.StartsWith("UPDATE "))
            {
                switch (cleanedCommandText)
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
                        cmd.CommandText = "UPDATE \"UFUserSecurity\" SET \"ManageForms\" = @p0, \"ManageDataSources\" = @p1, \"ManagePreValueSources\" = @p2, \"ManageWorkflows\" = @p3, \"ViewEntries\" = @p4, \"EditEntries\" = @p5, \"DeleteEntries\" = @p6 WHERE \"User\"::integer = @p7";
                        break;
                    case "UPDATE \"UFUserFormSecurity\" SET HasAccess = @p0, SecurityType = @p1, AllowInEditor = @p2 WHERE \"user\" = @p3 AND form = @p4":
                        cmd.CommandText = "UPDATE \"UFUserFormSecurity\" SET \"HasAccess\" = @p0, \"SecurityType\" = @p1, \"AllowInEditor\" = @p2 WHERE \"User\"::integer = @p3 AND \"Form\" = @p4";
                        break;
                    default:
                        success = false;
                        break;
                }
            }
            else if (cleanedCommandText.StartsWith("DELETE "))
            {
                switch (cleanedCommandText)
                {
                    case "DELETE FROM \"UFUserStartFolders\" WHERE UserId = @p0":
                        cmd.CommandText = "DELETE FROM \"UFUserStartFolders\" WHERE \"UserId\" = @p0";
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

            success = success && ConvertParameters(cmd);

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
                else if (parameter.DbType is DbType.DateTime)
                {

                }
            }

            return true;
        }

        private DateTime? EnsureUtc(object? value)
        {
            if (value is not DateTime dt)
            {
                return null;
            }

            DateTime rVar = dt;
            if (dt.Kind != DateTimeKind.Utc)
            {
                // PostgreSQL Npgsql expects DateTime to be in UTC
                rVar = dt.Kind == DateTimeKind.Local
                    ? dt.ToUniversalTime()
                    : dt.ToLocalTime().ToUniversalTime();
            }

            return rVar;
        }
    }
}
