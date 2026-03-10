using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Npgsql;

namespace Our.Umbraco.PostgreSql.Services
{
    public static class PostgreSqlExceptionExtensions
    {
        public static bool IsBusyOrLocked(this NpgsqlException ex) =>
        ex.ErrorCode.ToString()
            is PostgresErrorCodes.DeadlockDetected
            or PostgresErrorCodes.AdminShutdown
            or PostgresErrorCodes.ActiveSqlTransaction
            or PostgresErrorCodes.TransactionRollback;
    }
}
