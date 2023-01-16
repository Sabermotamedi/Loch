using Loch.Shared.Models;
using Loch.Shared.Web.API.Security.Extensions;
using Microsoft.AspNetCore.Http;

namespace Loch.Shared.Web.API.Security.CurrentUser
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor) => this._httpContextAccessor = httpContextAccessor;

        public UserInfo UserInfo { get; set; }
        
        public async Task<UserInfo> GetUserInfo()
        {
            await this.SetUserInfo();
            return this.UserInfo;
        }

        public async Task SetUserInfo()
        {
            Loch.Shared.Web.API.Security.CurrentUser.CurrentUser currentUser = this;
            UserInfo userInfoAsync = await currentUser._httpContextAccessor.HttpContext.GetUserInfoAsync();
            if (userInfoAsync == null)
                throw new UnauthorizedAccessException("currentUser");
            currentUser.UserInfo = new UserInfo()
            {
                Id = userInfoAsync.Id,
                Username = userInfoAsync.Username,
                Fullname = userInfoAsync.Fullname
            };
        }
    }
}