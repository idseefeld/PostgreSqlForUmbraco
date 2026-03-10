using System;
using System.Collections.Generic;
using System.Text;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace Our.Umbraco.PostgreSql.EFCore
{
    public class PostgreSqlForNpgsqlOptionsExtension : NpgsqlOptionsExtension
    {
        public PostgreSqlForNpgsqlOptionsExtension() { }

        protected PostgreSqlForNpgsqlOptionsExtension(PostgreSqlForNpgsqlOptionsExtension copyFrom)
            : base(copyFrom)
        { }

        protected override NpgsqlOptionsExtension Clone() => new PostgreSqlForNpgsqlOptionsExtension(this);
    }
}
