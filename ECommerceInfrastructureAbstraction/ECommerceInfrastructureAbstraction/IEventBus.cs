using ECommerceBusinessAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceInfrastructureAbstraction
{
    public interface IEventBus
    {
        void Publish<T>(T domainEvent) where T : IDomainEvent;

        void Subscribe<T, TH>()
        where T : IDomainEvent
        where TH : IDomainEventHandler<T>;

        void Unsubscribe<T, TH>()
        where TH : IDomainEventHandler<T>
        where T : IDomainEvent;
    }
}
