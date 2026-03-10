using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using Npgsql;
using Our.Umbraco.PostgreSql;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Our.Umbraco.PostgreSql.Services
{
    public class PostgreSqlBatchSqlInsertProvider : IBulkSqlInsertProvider
    {
        public string ProviderName => Constants.ProviderName;


        public int BulkInsertRecords<T>(IUmbracoDatabase database, IEnumerable<T> records)
        {
            T[] recordsA = [.. records];
            if (recordsA.Length == 0)
            {
                return 0;
            }

            PocoData? pocoData = database.PocoDataFactory.ForType(typeof(T))
                ?? throw new InvalidOperationException("Could not find PocoData for " + typeof(T));

            return BatchInsertRecordsPostgreSQL(database, pocoData, recordsA);
        }

        /// <summary>
        ///     Bulk-insert records using PostgreSQL COPY command for optimal performance.
        /// </summary>
        /// <typeparam name="T">The type of the records.</typeparam>
        /// <param name="database">The database.</param>
        /// <param name="pocoData">The POCO metadata.</param>
        /// <param name="records">The records.</param>
        /// <returns>The number of records that were inserted.</returns>
        private static int BatchInsertRecordsPostgreSQL<T>(IUmbracoDatabase database, PocoData pocoData, IEnumerable<T> records)
        {
            var recordsArray = records as T[] ?? records.ToArray();
            if (recordsArray.Length == 0)
            {
                return 0;
            }

            var inTrans = database.InTransaction;
            if (!inTrans)
            {
                database.BeginTransaction();
            }

            try
            {
                string tableName = pocoData.TableInfo.TableName;
                bool autoIncrement = pocoData.TableInfo.AutoIncrement;
                string? primaryKeyName = autoIncrement ? pocoData.TableInfo.PrimaryKey : null;

                // Get columns to insert (excluding auto-increment primary key)
                var columns = pocoData.Columns
                    .Where(c => !autoIncrement || !string.Equals(c.Key, primaryKeyName, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(c => c.Value.ColumnName)
                    .ToList();

                var connection = database.Connection as NpgsqlConnection;
                if (connection == null)
                {
                    // Fallback to individual inserts if we can't get NpgsqlConnection
                    return BulkInsertRecordsFallback(database, pocoData, recordsArray);
                }

                // Ensure connection is open
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                // Build COPY command
                var columnNames = string.Join(", ", columns.Select(c => $"\"{c.Value.ColumnName}\""));
                var copyCommand = $"COPY \"{tableName}\" ({columnNames}) FROM STDIN (FORMAT BINARY)";

                using var writer = connection.BeginBinaryImport(copyCommand);

                foreach (var record in recordsArray)
                {
                    writer.StartRow();
                    foreach (var column in columns)
                    {
                        var value = column.Value.GetValue(record);
                        if (value == null)
                        {
                            writer.WriteNull();
                        }
                        else
                        {
                            writer.Write(value);
                        }
                    }
                }

                var insertedCount = writer.Complete();

                if (!inTrans)
                {
                    database.CompleteTransaction();
                }

                return (int)insertedCount;
            }
            catch
            {
                if (!inTrans)
                {
                    database.AbortTransaction();
                }
                throw;
            }
        }

        /// <summary>
        ///     Fallback method using individual inserts when binary import is not available.
        /// </summary>
        private static int BulkInsertRecordsFallback<T>(IUmbracoDatabase database, PocoData pocoData, T[] records)
        {
            string tableName = pocoData.TableInfo.TableName;
            bool autoIncrement = pocoData.TableInfo.AutoIncrement;
            var primaryKeyName = autoIncrement ? pocoData.TableInfo.PrimaryKey : null;

            var count = 0;
            foreach (T record in records)
            {
                database.Insert(tableName, primaryKeyName!, autoIncrement, record);
                count++;
            }

            return count;
        }
    }
}
