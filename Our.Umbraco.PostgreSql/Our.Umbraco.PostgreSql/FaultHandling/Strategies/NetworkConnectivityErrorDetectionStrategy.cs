using Npgsql;
using System;
using Umbraco.Cms.Infrastructure.Persistence.FaultHandling;

namespace Our.Umbraco.PostgreSql.FaultHandling.Strategies;

/// <summary>
///     Implements a strategy that detects network connectivity errors such as host not found.
/// </summary>
public class NetworkConnectivityErrorDetectionStrategy : ITransientErrorDetectionStrategy
{
    public bool IsTransient(Exception? ex)
    {
        if (ex != null && ex is NpgsqlException sqlException)
        {
            switch (sqlException.SqlState)
            {
                case "25P02":
                    // 25P02: aktuelle Transaktion wurde abgebrochen, Befehle werden bis zum Ende der Transaktion ignoriert
                    return true;
                case "42703":
                    // 42703: Spalte »ID« existiert nicht
                    // throw ex;
                    break;
            }
        }

        return false;
    }
}
