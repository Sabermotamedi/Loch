// -----------------------------------------------------------------------
// <copyright file="RealPerson.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------


namespace Loch.Shared.Web.API.Security.Models
{
    internal class RealPerson
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string FullName { get { return FirstName + ' ' + LastName; } }
    }
}