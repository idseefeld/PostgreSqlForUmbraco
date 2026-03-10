using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Npgsql;
using Umbraco.Extensions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Our.Umbraco.PostgreSql.EFCore.Interceptors;

/// <summary>
/// Interceptor für das Abfangen von DbCommand-Exceptions bei PostgreSQL/Npgsql.
/// </summary>
public class DbCommandInterceptor : Microsoft.EntityFrameworkCore.Diagnostics.DbCommandInterceptor
{
    public override DbCommand CommandInitialized(CommandEndEventData eventData, DbCommand result)
    {
        try
        {
            return base.CommandInitialized(eventData, result);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public override InterceptionResult<DbCommand> CommandCreating(CommandCorrelatedEventData eventData, InterceptionResult<DbCommand> result)
    {
        try
        {
            return base.CommandCreating(eventData, result);
        }
        catch (Exception)
        {
            if (!result.Result.CommandText.InvariantContains("__EFMigrationsHistory"))
            {
                throw;
            }

            return result;
        }
    }

    public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
    {
        try
        {
            return base.CommandCreated(eventData, result);
        }
        catch (Exception)
        {
            if (!result.CommandText.InvariantContains("__EFMigrationsHistory"))
            {
                throw;
            }

            // Return the original result when exception is handled
            return result;
        }
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        try
        {
            return base.ReaderExecuting(command, eventData, result);
        }
        catch (Exception)
        {
            if (!command.CommandText.InvariantContains("__EFMigrationsHistory"))
            {
                throw;
            }

            // Return the original result wrapped in ValueTask when exception is handled
            return result;
        }
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
    {
        try
        {
            var awaitResult = base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
            var dataReader = awaitResult.Result;
            var dataTable =dataReader?.GetSchemaTable(); // Access schema to ensure reader is fully initialized

            return awaitResult;
        }
        catch (Exception)
        {
            if (!command.CommandText.InvariantContains("__EFMigrationsHistory"))
            {
                throw;
            }

            // Return the original result wrapped in ValueTask when exception is handled
            return new ValueTask<DbDataReader>(result);
        }
    }

    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
        if (!HandleException(command, eventData))
        {
            base.CommandFailed(command, eventData);
        }
    }

    public override Task CommandFailedAsync(
        DbCommand command,
        CommandErrorEventData eventData,
        CancellationToken cancellationToken = default)
    {
        if (!HandleException(command, eventData))
        {
            return base.CommandFailedAsync(command, eventData, cancellationToken);
        }

        return Task.CompletedTask;
    }

    private static bool HandleException(DbCommand command, CommandErrorEventData eventData)
    {
        bool handled = false;
        // PostgreSQL-spezifische Exception-Behandlung
        if (eventData.Exception is PostgresException pgEx)
        {
            // PostgresException enthält detaillierte PostgreSQL-Fehlercodes
            // Siehe: https://www.postgresql.org/docs/current/errcodes-appendix.html
            var sqlState = pgEx.SqlState;       // z.B. "23505" für unique_violation
            var errorCode = pgEx.ErrorCode;     // PostgreSQL error code
            var detail = pgEx.Detail;           // Zusätzliche Details
            var hint = pgEx.Hint;               // Lösungshinweis
            var tableName = pgEx.TableName;     // Betroffene Tabelle
            var columnName = pgEx.ColumnName;   // Betroffene Spalte
            var constraintName = pgEx.ConstraintName; // Name des Constraints


            if (sqlState == PostgresErrorCodes.UndefinedTable)
            {
                handled = true;
                if (command.CommandText.InvariantContains("__EFMigrationsHistory"))
                {
                    // Behandlung für nicht vorhandene Tabelle
                }

            }
        }
        else if (eventData.Exception is NpgsqlException npgsqlEx)
        {
            // Allgemeine Npgsql-Exceptions (z.B. Verbindungsprobleme)
            var innerException = npgsqlEx.InnerException;
        }

        // Logging oder weitere Verarbeitung
        LogException(command, eventData);

        return handled;
    }

    private static void LogException(DbCommand command, CommandErrorEventData eventData)
    {
        // Hier können Sie loggen oder die Exception transformieren
        Console.WriteLine($"SQL Failed: {command.CommandText}");
        Console.WriteLine($"Exception: {eventData.Exception.Message}");
        Console.WriteLine($"Duration: {eventData.Duration}");
    }
}
