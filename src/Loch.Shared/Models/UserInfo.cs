// -----------------------------------------------------------------------
// <copyright file="UserInfo.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

namespace Loch.Shared.Models
{
    /// <summary>
    /// Every thing that you need to know about user.
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// Gets or sets the Id of the User, Most of time you need to get.
        /// </summary>
        public Guid Id { get; set; }

        public string Fullname { get; set; }

        public string Username { get; set; }

        public long BusinessId { get; set; }

        public Guid BizDomainId { get; set; }
    }
}