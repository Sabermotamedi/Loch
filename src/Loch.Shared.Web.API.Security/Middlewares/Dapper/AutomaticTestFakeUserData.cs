// -----------------------------------------------------------------------
// <copyright file="AutomaticTestFakeUserData.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Loch.Shared.Models;

namespace Loch.Shared.Web.API.Security.Middlewares.Dapper
{
    public class AutomaticTestFakeUserData : IAutomaticTestFakeUserData
    {
        public AutomaticTestFakeUserData(UserInfo currentUserInfo) => CurrentUserInfo = currentUserInfo ?? throw new ArgumentNullException(nameof(currentUserInfo));

        public UserInfo CurrentUserInfo { get; set; }
    }
}