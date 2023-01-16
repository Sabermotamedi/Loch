// -----------------------------------------------------------------------
// <copyright file="Company.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

namespace Loch.Shared.Web.API.Security.Models
{
    internal class Company
    {
        public Guid? Id { get; set; }

        public string Symbol { get; set; }

        public Guid? IndustryId { get; set; }

        public Guid? StateId { get; set; }
    }
}