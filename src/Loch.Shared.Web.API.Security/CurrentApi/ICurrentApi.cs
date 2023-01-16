using Loch.Shared.Models;

namespace Loch.Shared.Web.API.Security.CurrentApi;

public interface ICurrentApi
{
    Task<CrmApiKey> GetUserInfoByApikey();

}