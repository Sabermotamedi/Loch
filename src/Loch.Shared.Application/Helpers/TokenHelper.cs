using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Loch.Shared.Application.Dtos.Token;

namespace Loch.Shared.Application.Helpers;
public class TokenHelper
{
    public List<string> GetRoles(ClaimsPrincipal user)
    {
        return user.Claims.Where(m => m.Type.Contains("role")).Select(m => m.Value).ToList();
    }

    public long GetId(ClaimsPrincipal user)
    {
        return Convert.ToInt64(user.Claims.FirstOrDefault(m => m.Type.Contains("nameidentifier"))?.Value);
    }

    public List<TokenClaimsDto> GetClaims(ClaimsPrincipal user)
    {
        return user.Claims.Select(m => new TokenClaimsDto { Type = m.Type, Value = m.Value }).ToList();
    }
}
