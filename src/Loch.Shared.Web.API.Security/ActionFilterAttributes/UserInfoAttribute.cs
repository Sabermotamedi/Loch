// -----------------------------------------------------------------------
// <copyright file="UserInfoAttribute.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using Loch.Shared.Models;
using Loch.Shared.Web.API.Security.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace Loch.Shared.Web.API.Security.ActionFilterAttributes
{
    public class UserInfoAttribute : ActionFilterAttribute, IAsyncActionFilter
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var currentUser = await context.HttpContext.GetUserInfoAsync();

            if (currentUser == null)
            {
                throw new UnauthorizedAccessException(nameof(currentUser));
            }

            var args = context.ActionArguments;

            var user = new UserInfo
            {
                Id = currentUser.Id,
                Username = currentUser.Username,
                Fullname = currentUser.Fullname                
            };

            foreach (var arg in args)
            {
                foreach (PropertyInfo propertyInfo in arg.Value.GetType().GetProperties())
                {
                    if (propertyInfo.PropertyType == typeof(UserInfo))
                    {
                        propertyInfo.SetValue(arg.Value, user, null);
                    }
                }
            }

            // execute any code before the action executes
            var result = await next();
            // execute any code after the action executes
        }
    }
}