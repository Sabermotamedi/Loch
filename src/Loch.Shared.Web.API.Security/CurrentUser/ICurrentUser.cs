using Loch.Shared.Models;

namespace Loch.Shared.Web.API.Security.CurrentUser
{
    public interface ICurrentUser
    {
        Task<UserInfo> GetUserInfo();
    }
}
