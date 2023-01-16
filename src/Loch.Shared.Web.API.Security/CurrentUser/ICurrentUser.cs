// -----------------------------------------------------------------------
// <copyright file="ICurrentUser.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using Loch.Shared.Models;

namespace Loch.Shared.Web.API.Security.CurrentUser
{
    public interface ICurrentUser
    {
        Task<UserInfo> GetUserInfo();
    }
}
