using AutoMapper;
using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessAbstraction;
using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccess.OrderRepository
{
    public class OrderRepository : IOrderRepository<OrderDataDto>
    {
        private readonly ECommerceDbContext context;
        private IMapper mapper;

        public OrderRepository(ECommerceDbContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public Task AddAsync(OrderDataDto entity)
        {
            throw new NotImplementedException();
        }

        public async Task AddOrder(OrderDataDto entity)
        {
            Order order  = mapper.Map<Order>(entity);
            await context.AddAsync(order);
        }

        public Task DeleteAsync(OrderDataDto entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrderDataDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<OrderDataDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(OrderDataDto entity)
        {
            throw new NotImplementedException();
        }
    }
}
