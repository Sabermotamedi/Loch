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