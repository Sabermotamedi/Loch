using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using Loch.Shared.Application.Dtos.Token;
using Microsoft.Extensions.Caching.Distributed;

namespace Loch.Shared.Application.Helpers;

public class AuthorizationHelper
{
    private readonly IDistributedCache _cache;
    private readonly TokenHelper _tokenHelper = new();
    private readonly HashingHelper _hashingHelper = new();

    public AuthorizationHelper(IDistributedCache cache)
    {
        _cache = cache;
    }

    public UserPermissionsDto GetDataFromRedis(ClaimsPrincipal claimsPrincipal)
    {
        var userId = _tokenHelper.GetId(claimsPrincipal);
        var claims = _tokenHelper.GetClaims(claimsPrincipal);
        var companyId = claims.FirstOrDefault(m => m.Type == "CompanyId")?.Value;
        var businessId = claims.FirstOrDefault(m => m.Type == "BusinessId")?.Value;
        var redisKey = _hashingHelper.HashKeyGenerate("Operation", companyId, businessId, userId);
        var data = _cache.GetString(redisKey);
        return data != null ? JsonSerializer.Deserialize<UserPermissionsDto>(data) : null;
    }
}