using Loch.Shared.Models;
using Loch.Shared.Web.API.Security.Extensions;
using Microsoft.AspNetCore.Http;

namespace Loch.Shared.Web.API.Security.CurrentApi;

public class CurrentApi : ICurrentApi
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentApi(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public CrmApiKey CrmApiKey { get; set; }

    public async Task<CrmApiKey> GetUserInfoByApikey()
    {
        await this.SetUserInfoByApikey();
        return this.CrmApiKey;
    }
    public async Task SetUserInfoByApikey()
    {
        Loch.Shared.Web.API.Security.CurrentApi.CurrentApi currentApi = this;

        CrmApiKey crmApiKeyAsync = await currentApi._httpContextAccessor.HttpContext.GetCrmApiKeyByApikeyAsync();
        if (crmApiKeyAsync == null)
            throw new UnauthorizedAccessException("currentUser");

        currentApi.CrmApiKey = crmApiKeyAsync;
        //{
        //    Id = crmApiKeyAsync.Id,
        //    BizdomainId = crmApiKeyAsync.BizdomainId,
        //    CreatorId = crmApiKeyAsync.CreatorId,
        //    IsDeleted = crmApiKeyAsync.IsDeleted,
        //    IsDisabled = crmApiKeyAsync.IsDisabled,
        //    LastAccessTime = crmApiKeyAsync.LastAccessTime,
        //    PermissionId = crmApiKeyAsync.PermissionId,
        //    RegisterTime = crmApiKeyAsync.RegisterTime
        //};
    }
}