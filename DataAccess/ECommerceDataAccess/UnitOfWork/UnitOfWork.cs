using AutoMapper;
using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessAbstraction;
using ECommerceDataAccessDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccess.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public IOrderRepository<OrderDataDto> OrderRepository { get; private set; }
        public IProductRepository<ProductDataDto> ProductRepository { get; private set; }

        private readonly ECommerceDbContext context;


        public UnitOfWork(ECommerceDbContext eCommerceDbContext,IProductRepository<ProductDataDto> productRepository,IOrderRepository<OrderDataDto> orderRepository)
        {
            this.context = eCommerceDbContext;
            this.ProductRepository = productRepository;
            this.OrderRepository = orderRepository;
        }
        public async Task<int> Complete()
        {
            return await context.SaveChangesAsync();
        }
    }
}
