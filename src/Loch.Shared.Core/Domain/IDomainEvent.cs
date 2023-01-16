// -----------------------------------------------------------------------
// <copyright file="IDomainEvent.cs" company="Loch">
// Copyright (c) Loch. All rights reserved.  Developed with 🖤 in development department.
// </copyright>
// -----------------------------------------------------------------------



using System;
using MediatR;

namespace Loch.Shared.Core.Domain
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }
}