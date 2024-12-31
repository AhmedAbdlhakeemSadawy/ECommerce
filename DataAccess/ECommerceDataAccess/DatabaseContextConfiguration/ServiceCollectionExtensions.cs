using ECommerceDataAccess.OrderRepository;
using ECommerceDataAccess.ProoductRepository;
using ECommerceDataAccessAbstraction;
using ECommerceDataAccessDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccess.DatabaseContextConfiguration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddECommerceDataAccess(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ECommerceDbContext>(options =>
                options.UseSqlServer(connectionString));
            services.AddScoped(typeof(IProductRepository<ProductDataDto>), typeof(ProductRepository));
            services.AddScoped(typeof(IOrderRepository<OrderDataDto>), typeof(OrderRepository.OrderRepository));
            return services;
        }
    }
}
