using System.Data.Common;

namespace Our.Umbraco.PostgreSql.Services
{
    public interface IPostgreSqlFixService
    {
        bool FixCommanText(DbCommand cmd);
    }
}
