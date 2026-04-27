using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Our.Umbraco.PostgreSql.EFCore.Services
{
    public class PostgreSqlDbContextOptions<TContext> : DbContextOptions<TContext>
       where TContext : DbContext
    {
    }
}
