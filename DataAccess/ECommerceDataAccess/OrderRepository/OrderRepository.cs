using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataEntities;
using ECommerceDataAccess.Mapping;
using ECommerceDataAccessAbstraction;
using ECommerceDataAccessDTO;
using Microsoft.EntityFrameworkCore;
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

        public OrderRepository(ECommerceDbContext context)
        {
            this.context = context;
        }

        public Task AddAsync(OrderDataDto entity)
        {
            throw new NotImplementedException();
        }

        public async Task AddOrder(OrderDataDto orderDataDto)
        {
            await context.AddAsync(orderDataDto.ToEntity());
        }

        public Task DeleteAsync(OrderDataDto entity)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OrderDataDto>> GetAllAsync()
        {
            var orders = await context.Orders.AsNoTracking().ToListAsync();
            return orders.ToDataDtos();
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
