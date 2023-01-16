// -----------------------------------------------------------------------
// <copyright file="IBusinessRule.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

namespace Loch.Shared.Core.Domain
{
    public interface IBusinessRule
    {
        bool IsBroken();

        Error Error { get; }
    }
}