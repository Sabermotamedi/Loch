// -----------------------------------------------------------------------
// <copyright file="PaginationRequest.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

namespace Loch.Shared.Queriess
{
    public class PaginationRequest : Request
    {
        public uint PageSize { get; set; } = 10;
        public uint PageIndex { get; set; } = 1;
    }
}
