// -----------------------------------------------------------------------
// <copyright file="Request.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using MediatR;
using Loch.Shared.Core.Application;
using Loch.Shared.Models;

namespace Loch.Shared.Queriess
{
    public class Request : IRequest<AppResult>
    {
        public UserInfo CurrentUserInfo { get; set; }
    }
}
