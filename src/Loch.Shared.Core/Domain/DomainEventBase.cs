// -----------------------------------------------------------------------
// <copyright file="DomainEventBase.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Loch.Shared.Core.Domain
{
    public class DomainEventBase : IDomainEvent
    {
        public DateTime OccurredOn { get; }

        public DomainEventBase()
        {
            this.OccurredOn = DateTime.UtcNow;
        }
    }
}