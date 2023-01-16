using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Loch.Shared.Core.Domain
{
    /// <summary>
    /// Base Entity.
    /// </summary>
    public abstract class Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        protected virtual object Actual => this;

        private List<IDomainEvent> _domainEvents;

        /// <summary>
        /// Gets Domain events occurred.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        protected Entity()
        {
            Id = 0;
        }

        /// <summary>
        /// Add domain event.
        /// </summary>
        /// <param name="domainEvent">DomainEvent.</param>
        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents ??= new List<IDomainEvent>();
            this._domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        protected void CheckRules(params IBusinessRule[] rules)
        {
            var errors = new List<Error>();

            foreach (var rule in rules)
            {
                if (rule.IsBroken())
                {
                    errors.Add(rule.Error);
                }
            }

            if (errors.Count > 0)
            {
                throw new BusinessRuleValidationException(errors);
            }
        }

        public override bool Equals(object obj)
        {
            // Reference equality means that two references refer to the same object in memory.
            // Identifier equality means that two different objects in memory refer to the same row in the database.
            if (!(obj is Entity other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (Actual.GetType() != other.Actual.GetType())
            {
                return false;
            }

            if (Id == 0 || other.Id == 0)
            {
                return false;
            }

            return Id == other.Id;
        }

        public static bool operator ==(Entity a, Entity b)
        {
            if (a is null && b is null)
            {
                return true;
            }

            if (a is null || b is null)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(Entity a, Entity b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (Actual.GetType().ToString() + Id).GetHashCode();
        }
    }
}
