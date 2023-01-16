using System.Data;

namespace Loch.Shared.Web.API.Security.Middlewares.Dapper
{
    public interface IAPISecurityDBConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}