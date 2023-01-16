using Loch.Shared.Application.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Loch.Shared.Application.Attributes;

public class AuthorizePermission : AuthorizeAttribute, IAuthorizationFilter
{
    public string[] Permissions { get; set; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var cache = context.HttpContext.RequestServices.GetService<IDistributedCache>();

        if (Permissions.ToList().Count == 0)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        var authorizationHelper = new AuthorizationHelper(cache);


        var data = authorizationHelper.GetDataFromRedis(context.HttpContext.User);

        var requiredPermissions = Permissions.ToList();


        if (data == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (data is { Permissions: { } })
        {
            if (data.Permissions.Any(xPermission => requiredPermissions.Any(m => m == xPermission)))
            {
                return;
            }
        }

        if (data is { PermissionGroups: { } })
        {
            if (data.PermissionGroups.Any(xPermission => requiredPermissions.Any(m => m == xPermission)))
            {
                return;
            }
        }

        context.Result = new UnauthorizedResult();
    }
}