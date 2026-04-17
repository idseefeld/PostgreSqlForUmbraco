using System.Data.Common;

namespace Our.Umbraco.PostgreSql.Services
{
    public interface IPostgreSqlFixService
    {
        bool FixCommanText(DbCommand cmd);

        Func<object, object>? GetParameterConverter(DbCommand dbCommand, Type sourceType);

        void InterceptCommandExecuting(DbCommand cmd);
    }
}
