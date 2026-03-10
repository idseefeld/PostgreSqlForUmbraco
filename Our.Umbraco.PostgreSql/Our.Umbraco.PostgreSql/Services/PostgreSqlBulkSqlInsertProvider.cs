using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPoco;
using Our.Umbraco.PostgreSql;
using Umbraco.Cms.Infrastructure.Persistence;

namespace Our.Umbraco.PostgreSql.Services
{
    public class PostgreSqlBulkSqlInsertProvider : IBulkSqlInsertProvider
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

            return BulkInsertRecordsPostgreSQL(database, pocoData, recordsA);
        }

        /// <summary>
        ///     Bulk-insert records using SqlServer BulkCopy method.
        /// </summary>
        /// <typeparam name="T">The type of the records.</typeparam>
        /// <param name="database">The database.</param>
        /// <param name="records">The records.</param>
        /// <returns>The number of records that were inserted.</returns>
        private static int BulkInsertRecordsPostgreSQL<T>(IUmbracoDatabase database, PocoData pocoData, IEnumerable<T> records)
        {
            var count = 0;
            var inTrans = database.InTransaction;

            if (!inTrans)
            {
                database.BeginTransaction();
            }

            string tableName = pocoData.TableInfo.TableName;
            bool autoIncrement = pocoData.TableInfo.AutoIncrement;

            var primaryKeyName = autoIncrement
                ? pocoData.TableInfo.PrimaryKey
                : null;

            foreach (T record in records)
            {
                try
                {
                    // NPoco Insert for PostgreSQL only returns nothing when primaryKey is null
#pragma warning disable CS8604 // Possible null reference argument.
                    _ = database.Insert(tableName, primaryKeyName, autoIncrement, record);
#pragma warning restore CS8604 // Possible null reference argument.
                }
                catch (Exception)
                {
                    throw;
                }

                count++;
            }

            if (!inTrans)
            {
                database.CompleteTransaction();
            }

            return count;
        }
    }
}
