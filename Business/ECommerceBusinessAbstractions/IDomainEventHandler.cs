using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceBusinessAbstractions
{
    public interface IDomainEventHandler<T> where T : IDomainEvent
    {
        Task Handle(T domainEvent);
    }
}
