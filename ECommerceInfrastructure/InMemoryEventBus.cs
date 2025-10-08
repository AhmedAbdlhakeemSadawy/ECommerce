using ECommerceBusinessAbstractions;
using ECommerceInfrastructureAbstraction;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceInfrastructure
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly IServiceProvider serviceProvider;

        public InMemoryEventBus(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public async Task Publish<T>(T domainEvent) where T : IDomainEvent
        {
            using var scope = serviceProvider.CreateScope();
            var handlers = scope.ServiceProvider.GetServices<IDomainEventHandler<T>>();
            foreach (var handler in handlers)
            {
                await handler.Handle(domainEvent);
            }
        }

        public void Subscribe<T, TH>()
            where T : IDomainEvent
            where TH : IDomainEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public void Unsubscribe<T, TH>()
            where T : IDomainEvent
            where TH : IDomainEventHandler<T>
        {
            throw new NotImplementedException();
        }
    }
}
