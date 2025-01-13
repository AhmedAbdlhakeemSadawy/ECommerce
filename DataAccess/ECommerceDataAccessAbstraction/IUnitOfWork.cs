using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccessAbstraction
{
    public interface IUnitOfWork
    {
        IProductRepository<ProductDataDto> ProductRepository { get; }
        IOrderRepository<OrderDataDto>  OrderRepository { get; }

        public Task<int> Complete();

    }
}
