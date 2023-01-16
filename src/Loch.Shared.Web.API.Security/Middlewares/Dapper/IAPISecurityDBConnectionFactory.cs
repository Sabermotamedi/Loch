// -----------------------------------------------------------------------
// <copyright file="IAPISecurityDBConnectionFactory.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using System.Data;

namespace Loch.Shared.Web.API.Security.Middlewares.Dapper
{
    public interface IAPISecurityDBConnectionFactory
    {
        Task<IDbConnection> CreateConnectionAsync();
    }
}