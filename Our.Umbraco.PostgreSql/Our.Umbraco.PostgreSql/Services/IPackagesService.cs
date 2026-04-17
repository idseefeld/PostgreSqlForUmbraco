using System.Data.Common;

namespace Our.Umbraco.PostgreSql.Services
{
    public interface IPackagesService
    {
        DbCommand FixCommanText(DbCommand cmd);

        void InterceptCommandExecuting(DbCommand cmd);
    }
}
