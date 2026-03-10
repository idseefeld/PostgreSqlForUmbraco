using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Our.Umbraco.PostgreSql.EFCore.Interceptors;

namespace Our.Umbraco.PostgreSql.EFCore
{
    public static class PostgreSqlDbContextOptionsBuilderExtensions
    {
        /// <summary>
        ///     <para>
        ///         Configures the context to connect to a PostgreSQL server with Npgsql, but without initially setting any
        ///         <see cref="DbConnection" /> or connection string.
        ///     </para>
        ///     <para>
        ///         The connection or connection string must be set before the <see cref="DbContext" /> is used to connect
        ///         to a database. Set a connection using <see cref="RelationalDatabaseFacadeExtensions.SetDbConnection" />.
        ///         Set a connection string using <see cref="RelationalDatabaseFacadeExtensions.SetConnectionString" />.
        ///     </para>
        /// </summary>
        /// <param name="optionsBuilder">The builder being used to configure the context.</param>
        /// <param name="npgsqlOptionsAction">An optional action to allow additional Npgsql-specific configuration.</param>
        /// <returns>The options builder so that further configuration can be chained.</returns>
        public static DbContextOptionsBuilder UsePostgreSql(
            this DbContextOptionsBuilder optionsBuilder,
            Action<PostgreSqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(GetOrCreateExtension(optionsBuilder));

            ConfigureDbCommandInterceptors(optionsBuilder);

            ConfigureWarnings(optionsBuilder);

            npgsqlOptionsAction?.Invoke(new PostgreSqlDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     Configures the context to connect to a PostgreSQL database with Npgsql.
        /// </summary>
        /// <param name="optionsBuilder">A builder for setting options on the context.</param>
        /// <param name="connectionString">The connection string of the database to connect to.</param>
        /// <param name="npgsqlOptionsAction">An optional action to allow additional Npgsql-specific configuration.</param>
        /// <returns>
        ///     The options builder so that further configuration can be chained.
        /// </returns>
        public static DbContextOptionsBuilder UsePostgreSql(
            this DbContextOptionsBuilder optionsBuilder,
            string? connectionString,
            Action<PostgreSqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));

            var extension = (NpgsqlOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnectionString(connectionString);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            ConfigureDbCommandInterceptors(optionsBuilder);

            ConfigureWarnings(optionsBuilder);

            npgsqlOptionsAction?.Invoke(new PostgreSqlDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     Configures the context to connect to a PostgreSQL database with Npgsql.
        /// </summary>
        /// <param name="optionsBuilder">The builder being used to configure the context.</param>
        /// <param name="connection">
        ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
        ///     in the open state then EF will not open or close the connection. If the connection is in the closed
        ///     state then EF will open and close the connection as needed. The caller owns the connection and is
        ///     responsible for its disposal.
        /// </param>
        /// <param name="npgsqlOptionsAction">An optional action to allow additional Npgsql-specific configuration.</param>
        /// <returns>The options builder so that further configuration can be chained.</returns>
        public static DbContextOptionsBuilder UsePostgreSql(
            this DbContextOptionsBuilder optionsBuilder,
            DbConnection connection,
            Action<PostgreSqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
            => UsePostgreSql(optionsBuilder, connection, contextOwnsConnection: false, npgsqlOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a PostgreSQL database with Npgsql.
        /// </summary>
        /// <param name="optionsBuilder">A builder for setting options on the context.</param>
        /// <param name="connection">
        ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
        ///     in the open state then EF will not open or close the connection. If the connection is in the closed
        ///     state then EF will open and close the connection as needed.
        /// </param>
        /// <param name="contextOwnsConnection">
        ///     If <see langword="true" />, then EF will take ownership of the connection and will
        ///     dispose it in the same way it would dispose a connection created by EF. If <see langword="false" />, then the caller still
        ///     owns the connection and is responsible for its disposal.
        /// </param>
        /// <param name="npgsqlOptionsAction">An optional action to allow additional Npgsql-specific configuration.</param>
        /// <returns>
        ///     The options builder so that further configuration can be chained.
        /// </returns>
        public static DbContextOptionsBuilder UsePostgreSql(
            this DbContextOptionsBuilder optionsBuilder,
            DbConnection connection,
            bool contextOwnsConnection,
            Action<PostgreSqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotNull(connection, nameof(connection));

            var extension = (NpgsqlOptionsExtension)GetOrCreateExtension(optionsBuilder).WithConnection(connection, contextOwnsConnection);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            ConfigureDbCommandInterceptors(optionsBuilder);

            ConfigureWarnings(optionsBuilder);

            npgsqlOptionsAction?.Invoke(new PostgreSqlDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     Configures the context to connect to a PostgreSQL database with Npgsql.
        /// </summary>
        /// <param name="optionsBuilder">A builder for setting options on the context.</param>
        /// <param name="dataSource">A <see cref="DbDataSource" /> which will be used to get database connections.</param>
        /// <param name="npgsqlOptionsAction">An optional action to allow additional Npgsql-specific configuration.</param>
        /// <returns>
        ///     The options builder so that further configuration can be chained.
        /// </returns>
        public static DbContextOptionsBuilder UsePostgreSql(
            this DbContextOptionsBuilder optionsBuilder,
            DbDataSource dataSource,
            Action<PostgreSqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
        {
            Check.NotNull(optionsBuilder, nameof(optionsBuilder));
            Check.NotNull(dataSource, nameof(dataSource));

            var extension = (NpgsqlOptionsExtension)GetOrCreateExtension(optionsBuilder).WithDataSource(dataSource);
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            ConfigureDbCommandInterceptors(optionsBuilder);

            ConfigureWarnings(optionsBuilder);

            npgsqlOptionsAction?.Invoke(new PostgreSqlDbContextOptionsBuilder(optionsBuilder));

            return optionsBuilder;
        }

        /// <summary>
        ///     <para>
        ///         Configures the context to connect to a PostgreSQL server with Npgsql, but without initially setting any
        ///         <see cref="DbConnection" />, <see cref="DbDataSource" /> or connection string.
        ///     </para>
        ///     <para>
        ///         The connection, data source or connection string must be set explicitly or registered in the DI
        ///         before the <see cref="DbContext" /> is used to connect to a database.
        ///         Set a connection using <see cref="RelationalDatabaseFacadeExtensions.SetDbConnection" />, a data source using
        ///         <see cref="NpgsqlDatabaseFacadeExtensions.SetDbDataSource" />, or a connection string using
        ///         <see cref="RelationalDatabaseFacadeExtensions.SetConnectionString" />.
        ///     </para>
        /// </summary>
        /// <param name="optionsBuilder">The builder being used to configure the context.</param>
        /// <param name="npgsqlOptionsAction">An optional action to allow additional Npgsql-specific configuration.</param>
        /// <returns>The options builder so that further configuration can be chained.</returns>
        public static DbContextOptionsBuilder<TContext> UsePostgreSql<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder,
            Action<PostgreSqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UsePostgreSql(
                (DbContextOptionsBuilder)optionsBuilder, npgsqlOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a PostgreSQL database with Npgsql.
        /// </summary>
        /// <param name="optionsBuilder">A builder for setting options on the context.</param>
        /// <param name="connectionString">The connection string of the database to connect to.</param>
        /// <param name="npgsqlOptionsAction">An optional action to allow additional Npgsql-configuration.</param>
        /// <returns>
        ///     The options builder so that further configuration can be chained.
        /// </returns>
        public static DbContextOptionsBuilder<TContext> UsePostgreSql<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder,
            string? connectionString,
            Action<PostgreSqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UsePostgreSql(
                (DbContextOptionsBuilder)optionsBuilder, connectionString, npgsqlOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a PostgreSQL database with Npgsql.
        /// </summary>
        /// <param name="optionsBuilder">A builder for setting options on the context.</param>
        /// <param name="connection">
        ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
        ///     in the open state then EF will not open or close the connection. If the connection is in the closed
        ///     state then EF will open and close the connection as needed. The caller owns the connection and is
        ///     responsible for its disposal.
        /// </param>
        /// <param name="npgsqlOptionsAction">An optional action to allow additional Npgsql-specific configuration.</param>
        /// <returns>
        ///     The options builder so that further configuration can be chained.
        /// </returns>
        public static DbContextOptionsBuilder<TContext> UsePostgreSql<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder,
            DbConnection connection,
            Action<PostgreSqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UsePostgreSql(
                (DbContextOptionsBuilder)optionsBuilder, connection, npgsqlOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a PostgreSQL database with Npgsql.
        /// </summary>
        /// <typeparam name="TContext">The type of context to be configured.</typeparam>
        /// <param name="optionsBuilder">The builder being used to configure the context.</param>
        /// <param name="connection">
        ///     An existing <see cref="DbConnection" /> to be used to connect to the database. If the connection is
        ///     in the open state then EF will not open or close the connection. If the connection is in the closed
        ///     state then EF will open and close the connection as needed.
        /// </param>
        /// <param name="contextOwnsConnection">
        ///     If <see langword="true" />, then EF will take ownership of the connection and will
        ///     dispose it in the same way it would dispose a connection created by EF. If <see langword="false" />, then the caller still
        ///     owns the connection and is responsible for its disposal.
        /// </param>
        /// <param name="npgsqlOptionsAction">An optional action to allow additional Npgsql-specific configuration.</param>
        /// <returns>The options builder so that further configuration can be chained.</returns>
        public static DbContextOptionsBuilder<TContext> UsePostgreSql<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder,
            DbConnection connection,
            bool contextOwnsConnection,
            Action<PostgreSqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UsePostgreSql(
                (DbContextOptionsBuilder)optionsBuilder, connection, contextOwnsConnection, npgsqlOptionsAction);

        /// <summary>
        ///     Configures the context to connect to a PostgreSQL database with Npgsql.
        /// </summary>
        /// <param name="optionsBuilder">A builder for setting options on the context.</param>
        /// <param name="dataSource">A <see cref="DbDataSource" /> which will be used to get database connections.</param>
        /// <param name="npgsqlOptionsAction">An optional action to allow additional Npgsql-specific configuration.</param>
        /// <returns>
        ///     The options builder so that further configuration can be chained.
        /// </returns>
        public static DbContextOptionsBuilder<TContext> UsePostgreSql<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder,
            DbDataSource dataSource,
            Action<PostgreSqlDbContextOptionsBuilder>? npgsqlOptionsAction = null)
            where TContext : DbContext
            => (DbContextOptionsBuilder<TContext>)UsePostgreSql(
                (DbContextOptionsBuilder)optionsBuilder, dataSource, npgsqlOptionsAction);

        /// <summary>
        ///     Returns an existing instance of <see cref="NpgsqlOptionsExtension" />, or a new instance if one does not exist.
        /// </summary>
        /// <param name="optionsBuilder">The <see cref="DbContextOptionsBuilder" /> to search.</param>
        /// <returns>
        ///     An existing instance of <see cref="NpgsqlOptionsExtension" />, or a new instance if one does not exist.
        /// </returns>
        private static PostgreSqlForNpgsqlOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.Options.FindExtension<PostgreSqlForNpgsqlOptionsExtension>() is { } existing
                ? existing
                : new PostgreSqlForNpgsqlOptionsExtension();

        private static void ConfigureDbCommandInterceptors(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(new DbCommandInterceptor());
        }

        private static void ConfigureWarnings(DbContextOptionsBuilder optionsBuilder)
        {

            var coreOptionsExtension = optionsBuilder.Options.FindExtension<CoreOptionsExtension>()
                ?? new CoreOptionsExtension();

            coreOptionsExtension = RelationalOptionsExtension.WithDefaultWarningConfiguration(coreOptionsExtension);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(coreOptionsExtension);
        }
    }

    // Add this utility class to the file (or preferably to a shared location in your project)
    internal static class Check
    {
        public static T NotNull<T>(T value, string parameterName)
            where T : class
        {
            if (value is null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }
    }
}
