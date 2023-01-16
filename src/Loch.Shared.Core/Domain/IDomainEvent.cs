using System;
using MediatR;

namespace Loch.Shared.Core.Domain
{
    public interface IDomainEvent : INotification
    {
        DateTime OccurredOn { get; }
    }
}