namespace Our.Umbraco.PostgreSql.EFCore
{
    /// <summary>
    ///     Constants related to PostgreSQL.
    /// </summary>
    public static class Constants
    {
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
        /// Default database name for Umbraco.
        /// </summary>U
        public const string UmbracoDefaultDatabaseName = "Umbraco";
    }
}
