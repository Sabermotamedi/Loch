// -----------------------------------------------------------------------
// <copyright file="IMessage.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

namespace Loch.Shared.Core.Application
{
    public interface IMessage
    {
        Dictionary<long, string> Messages { get; }
    }
}