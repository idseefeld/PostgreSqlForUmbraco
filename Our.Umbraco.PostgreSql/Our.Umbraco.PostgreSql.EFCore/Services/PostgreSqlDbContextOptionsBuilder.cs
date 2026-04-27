using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Our.Umbraco.PostgreSql.EFCore.Extensions;

namespace Our.Umbraco.PostgreSql.EFCore
{
    public class PostgreSqlDbContextOptionsBuilder : RelationalDbContextOptionsBuilder<PostgreSqlDbContextOptionsBuilder, PostgreSqlForNpgsqlOptionsExtension>
    {

        public PostgreSqlDbContextOptionsBuilder(DbContextOptionsBuilder optionsBuilder)
            : base(optionsBuilder)
        { }
    }
}
