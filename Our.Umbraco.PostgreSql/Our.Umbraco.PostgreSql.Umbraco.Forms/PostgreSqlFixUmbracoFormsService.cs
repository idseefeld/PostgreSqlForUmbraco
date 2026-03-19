using System.Data;
using System.Data.Common;
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
        public override bool FixCommanText(DbCommand cmd) => FixUmbracoFormsIssues(cmd);

        private bool FixUmbracoFormsIssues(DbCommand cmd)
        {
            var success = true;

            if (!(cmd.CommandText.Contains(" UF") || cmd.CommandText.Contains(" \"UF")))
            {
                return success;
            }

            var oldCommandText = cmd.CommandText;
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
                        cmd.CommandText = "SELECT COUNT(*) As \"Count\", MAX(\"Created\") As \"LastSubmittedDate\"\nFROM \"UFRecords\"\nWHERE (\"Created\" >= '@0' AND \"Created\" <= '@1')\nAND (\"Form\" = '@2')";
                        break;
                    //case "SELECT COUNT(*) As \"Count\", MAX(\"Created\") As \"LastSubmittedDate\"\nFROM \"UFRecords\"\nWHERE (\"Created\" >= '@0' AND \"Created\" <= '@1')\nAND (\"Form\" = '@2')":

                    //    break;
                    case "SELECT \"UFForms\".\"FolderKey\" AS \"FolderKey\", \"UFForms\".\"NodeId\" AS \"NodeId\", \"UFForms\".\"CreatedBy\" AS \"CreatedBy\", \"UFForms\".\"UpdatedBy\" AS \"UpdatedBy\", \"UFForms\".\"Id\" AS \"Id\", \"UFForms\".\"Key\" AS \"Key\", \"UFForms\".\"Name\" AS \"Name\", \"UFForms\".\"Definition\" AS \"Definition\", \"UFForms\".\"Created\" AS \"CreateDate\", \"UFForms\".\"Updated\" AS \"UpdateDate\"\nFROM \"UFForms\"\nWHERE ((\"UFForms\".\"Key\" = @p0))":
                        cmd.CommandText = "SELECT \"UFForms\".\"FolderKey\" AS \"FolderKey\", \"UFForms\".\"NodeId\" AS \"NodeId\", \"UFForms\".\"CreatedBy\" AS \"CreatedBy\", \"UFForms\".\"UpdatedBy\" AS \"UpdatedBy\", \"UFForms\".\"Id\" AS \"Id\", \"UFForms\".\"Key\" AS \"Key\", \"UFForms\".\"Name\" AS \"Name\", \"UFForms\".\"Definition\" AS \"Definition\", \"UFForms\".\"Created\" AS \"CreateDate\", \"UFForms\".\"Updated\" AS \"UpdateDate\"\nFROM \"UFForms\"\nWHERE ((\"UFForms\".\"Key\" = '@0'))";
                        break;
                    default:
                        success = false;
                        break;
                }
            }
            else if (cmd.CommandText.StartsWith("INSERT "))
            {
                switch (cmd.CommandText)
                {
                    case "INSERT INTO UFRecordDataString([Key], [Value]) VALUES(@p0, @p1)":
                        cmd.CommandText = "INSERT INTO \"UFRecordDataString\" (\"Key\", \"Value\") VALUES (@0, @1)";
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
                        cmd.CommandText = "UPDATE \"UFUserSecurity\" SET \"ManageForms\" = @0, \"ManageDataSources\" = @1, \"ManagePreValueSources\" = @2, \"ManageWorkflows\" = @3, \"ViewEntries\" = @4, \"EditEntries\" = @5, \"DeleteEntries\" = @6 WHERE \"User\" = '@7'";
                        break;
                    case "UPDATE \"UFUserSecurity\" SET manageforms = @0, managedatasources = @1, manageprevaluesources = @2, manageworkflows = @3, viewEntries = @4, editEntries = @5, deleteEntries = @6 WHERE [user] = @7":
                        cmd.CommandText = "UPDATE \"UFUserSecurity\" SET \"ManageForms\" = @0, \"ManageDataSources\" = @1, \"ManagePreValueSources\" = @2, \"ManageWorkflows\" = @3, \"ViewEntries\" = @4, \"EditEntries\" = @5, \"DeleteEntries\" = @6 WHERE \"User\" = '@7'";
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
