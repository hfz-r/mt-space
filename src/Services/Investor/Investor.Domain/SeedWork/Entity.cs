using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;

namespace AHAM.Services.Investor.Domain.SeedWork
{
    public abstract class Entity
    {
        private int? _requestedHashCode;
        private List<INotification> _domainEvents;

        public virtual int Id { get; protected set; }

        [JsonIgnore]
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            return Id == default;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity)) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (GetType() != obj.GetType()) return false;

            Entity item = (Entity) obj;

            if (item.IsTransient() || IsTransient()) return false;

            return item.Id == Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                _requestedHashCode ??= Id.GetHashCode() ^ 31;

                return _requestedHashCode.Value;
            }

            return base.GetHashCode();
        }

        public static bool operator ==(Entity left, Entity right)
        {
            return left?.Equals(right) ?? Equals(right, null);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}