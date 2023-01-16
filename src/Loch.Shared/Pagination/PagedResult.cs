// -----------------------------------------------------------------------
// <copyright file="PagedResult.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Loch.Shared.Pagination
{
    public class PagedResult<T> : PagedResultBase
    {
        public IList<T> Results { get; set; }

        public PagedResult()
        {
            Results = new List<T>();
        }
    }
}