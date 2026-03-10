namespace Our.Umbraco.PostgreSql
{
    /// <summary>
    ///     Constants related to PostgreSQL.
    /// </summary>
    public static class Constants
    {
        public const bool EscapeTableColumAliasNames = true;

        /// <summary>
        /// Represents the configuration section name for PostgreSQL options.
        /// </summary>
        public const string Configuration = "PostgreSqlOptions";

        /// <summary>
        ///     PostgreSQL provider name.
        /// </summary>
        public const string ProviderName = "Npgsql2";

        /// <summary>
        ///     PostgreSQL dbprovider name.
        /// </summary>
        public const string DbProvider = "Npgsql2";
        /// <summary>
        /// PostgreSQL name
        /// </summary>
        public const string Name = "PostgreSql";

        /// <summary>
        /// Connect to default db to create new one.
        /// </summary>
        public const string PostgreSqlDefaultDatabase = "postgres";

        /// <summary>
        public const string DefaultTimeZone = "W. Europe Standard Time";
        /// Default database name for Umbraco.
        /// </summary>U
        public const string UmbracoDefaultDatabaseName = "Umbraco";

        /// <summary>
        /// Specifies the default maximum number of retry attempts for test operations.
        /// </summary>
        public const int TestRetryMax = 10; // default is 10

        /// <summary>
        /// Represents the name of the foreign key constraint linking content version culture variations to content in
        /// the database.
        /// </summary>
        /// <remarks>This constant can be used when referencing the specific foreign key constraint in
        /// database operations, migrations, or for consistency across the codebase.</remarks>
        public const string FkContentVersionCultureVariationAndContent = "FK_umbracoContentVersionCultureVariation_umbracoContent_955834c7";
    }
}
