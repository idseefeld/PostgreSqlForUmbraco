using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Our.Umbraco.PostgreSql.EFCore
{
    public class PostgreSqlDbContextOptionsBuilder : RelationalDbContextOptionsBuilder<PostgreSqlDbContextOptionsBuilder, PostgreSqlForNpgsqlOptionsExtension>
    {

        public PostgreSqlDbContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder)
        { }
    }
}
