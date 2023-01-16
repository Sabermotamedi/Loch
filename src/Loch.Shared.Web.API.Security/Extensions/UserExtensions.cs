// -----------------------------------------------------------------------
// <copyright file="UserExtensions.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using Loch.Shared.Models;
using Loch.Shared.Web.API.Security.Middlewares.Dapper;
using Loch.Shared.Web.API.Security.Models;
using Loch.Shared.Web.API.Security.Utilities;
using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Logging;
using Microsoft.Net.Http.Headers;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Loch.Shared.Web.API.Security.Extensions
{
    public static class UserExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.Claims.SingleOrDefault(c => c.Type == "sub");

            if (userId != null)
            {
                return Guid.Parse(userId.Value);
            }
            else
            {
                return Guid.Empty;
            }
        }

        public static async Task<UserInfo> GetUserInfoAsync(this HttpContext context)
        {
            var fakeUserDataService = (AutomaticTestFakeUserData)context.RequestServices.GetService(typeof(IAutomaticTestFakeUserData));
            if (fakeUserDataService != null)
            {
                if (fakeUserDataService.CurrentUserInfo != null)
                {
                    return fakeUserDataService.CurrentUserInfo;
                }

                throw new Exception("CurrentUserInfo of AutomaticTestFakeUserData is Null.");
            }

            var userId = context.User.GetUserId();

            if (context.User == null)
            {
                throw new Exception("CurrentUserMiddleware should be used.");
            }

            if (userId == Guid.Empty)
            {
                return null;
            }

            var database = (IAPISecurityDBConnectionFactory)context.RequestServices.GetService(typeof(IAPISecurityDBConnectionFactory));

            using var conn = await database.CreateConnectionAsync();
            var db = new QueryFactory(conn, new SqlServerCompiler());

            RealPerson realPerson = await db.GetRealPerson(userId);

            var userInfo = new UserInfo
            {
                Id = realPerson.Id,
                Username = realPerson.Username,
                Fullname = realPerson.FullName
            };


            return userInfo;
        }

        public static async Task<CrmApiKey> GetCrmApiKeyByApikeyAsync(this HttpContext context)
        {
            //var fakeUserDataService = (AutomaticTestFakeUserData)context.RequestServices.GetService(typeof(IAutomaticTestFakeUserData));
            //if (fakeUserDataService != null)
            //{
            //    if (fakeUserDataService.CurrentUserInfo != null)
            //    {
            //        return fakeUserDataService.CurrentUserInfo;
            //    }

            //    throw new Exception("CurrentUserInfo of AutomaticTestFakeUserData is Null.");
            //}

            StringValues ApiKey = context.Request.Headers["XApiKey"];

            ApiKeyHash apiKeyHash = new ApiKeyHash();
            string ComputedApiKey = apiKeyHash.ComputeHash(ApiKey.ToString().ToLower());



            if (string.IsNullOrEmpty(ComputedApiKey))
            {
                return null;
            }

            var database = (IAPISecurityDBConnectionFactory)context.RequestServices.GetService(typeof(IAPISecurityDBConnectionFactory));

            using var conn = await database.CreateConnectionAsync();
            var db = new QueryFactory(conn, new SqlServerCompiler());

            var crmApiKey = await db.GetCrmApiKey(ComputedApiKey);

            if (crmApiKey == null)
            {
                return null;
            }

            return crmApiKey;
        }
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            var isAdmin = user.Claims.Any(c => c.Type == "role" && c.Value == "LochSSOAdministrator");

            return isAdmin;
        }

        public static ClaimsPrincipal GetClaimsPrincipal(this HttpRequest request)
        {
            IdentityModelEventSource.ShowPII = true;
            var tokenString = request.Headers[HeaderNames.Authorization].Single();
            var parts = tokenString.Split(" ");
            // var scheme = parts[0]; TODO : Check unnessesary assignment
            var credentials = parts[1];
            var jwt = new JwtSecurityToken(credentials);
            var claimsidentity = new ClaimsIdentity(
                jwt.Claims,
                IdentityServerAuthenticationDefaults.AuthenticationScheme,
                JwtClaimTypes.Name,
                JwtClaimTypes.Role);
            return new ClaimsPrincipal(claimsidentity);
        }

        public static async Task<IEnumerable<Permission>> GetUserPermissionsAsync(this HttpContext context)
        {
            Guid userId = context.User.GetUserId();

            if (userId == Guid.Empty)
            {
                return null;
            }

            var database = (IAPISecurityDBConnectionFactory)context.RequestServices.GetService(typeof(IAPISecurityDBConnectionFactory));
            using var conn = await database.CreateConnectionAsync();
            var db = new QueryFactory(conn, new SqlServerCompiler());

            var permissions = await db.GetUserPermissions(userId);

            return permissions;
        }
    }
}
