using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Loch.Shared.Web.API.Security.Extensions;

namespace Loch.Shared.Web.API.Security.Middlewares.Middlewares
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;
        public CurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.Keys.Contains(HeaderNames.Authorization))
            {
                context.User = context.Request.GetClaimsPrincipal();
            }

            await _next(context);
        }
    }
}