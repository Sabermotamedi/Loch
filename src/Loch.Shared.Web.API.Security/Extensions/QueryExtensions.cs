// -----------------------------------------------------------------------
// <copyright file="QueryExtensions.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using Loch.Shared.Models;
using Loch.Shared.Web.API.Security.Models;
using SqlKata.Execution;

namespace Loch.Shared.Web.API.Security.Extensions
{
    public static class QueryExtensions
    {
        internal static async Task<RealPerson> GetRealPerson(this QueryFactory db, Guid userId)
        {
            var realPerson = await db.Query("RealPerson")
            .Where("UserId", "=", userId)
            .Select("Id", "Username", "FirstName", "LastName")
            .FirstAsync<RealPerson>();

            return realPerson;
        }

        internal static async Task<CrmApiKey> GetCrmApiKey(this QueryFactory db, string apiKey)
        {
            var crmApiKey = await db.Query("secApiKey").Join("accBizDomain","accBizDomain.Id","secApiKey.BizdomainId")
            .Where("ApiKeyHash", "=", apiKey)
            .Select("secApiKey.Id as ApiKeyId", "BizdomainId", "PermissionId", "secApiKey.CreatorId", "secApiKey.IsDisabled", "IsDeleted", "secApiKey.RegisterTime", "secApiKey.LastAccessTime","accBizDomain.IsDisabled as AccBizDomainIsDisabled")
            .FirstAsync<CrmApiKey>();

            return crmApiKey;
        }

        internal static async Task<IEnumerable<Position>> GetActivePositionsByRealPersonId(this QueryFactory db, Guid realPersonId)
        {
            var positions = await db.Query("Position")
            .Join("PositionHistory", "PositionHistory.PositionId", "Position.Id")
            .Join("Post", "Post.Id", "Position.PostId")
            .Where("PositionHistory.PersonId", "=", realPersonId)
            .WhereNull("PositionHistory.EndDate")
            .Where("Position.IsActive", "=", 1)
            .Select("Position.Id", "Position.OrganizationUnitId", "Post.Name as PostName")
            .GetAsync<Position>();
            return positions;
        }

        internal static async Task<Company> GetCompanyByOrganizationUnitId(this QueryFactory db, Guid orgUnitId)
        {
            var company = await db.Query("Company")
                .Join("Organization", "Organization.LegalPersonId", "Company.LegalPersonId")
                .Join("OrganizationUnit", "OrganizationUnit.OrganizationId", "Organization.Id")
                .Where("OrganizationUnit.Id", "=", orgUnitId)
                .Select("Company.Id", "Company.IndustryId", "Company.Symbol", "Company.StateId").FirstAsync<Company>();
            return company;
        }

        internal static async Task<IEnumerable<Permission>> GetUserPermissions(this QueryFactory db, Guid userId)
        {
            var permissions = await db.Query("RealPerson")
            .Join("PositionHistory", "PositionHistory.PersonId", "RealPerson.Id")
            .Join("Position", "PositionHistory.PositionId", "Position.Id")
            .Join("PositionRole", "PositionRole.PositionId", "Position.Id")
            .Join("RolePermission", "RolePermission.RoleId", "PositionRole.RoleId")
            .Join("Permission", "RolePermission.PermissionId", "Permission.Id")
            .WhereNull("PositionHistory.EndDate")
            .Where("Position.IsActive", "=", 1)
            .Where("RealPerson.UserId", "=", userId)
            .Select("Permission.Id", "Permission.GrantedMicroPermissions", "Permission.DeniedMicroPermissions")
            .GetAsync<Permission>();

            return permissions;
        }
    }
}