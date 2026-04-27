using ECommerceDataAccess.OrderRepository;
using ECommerceDataAccess.ProoductRepository;
using ECommerceDataAccess.UnitOfWork;
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
                options.UseNpgsql(connectionString));
            services.AddScoped(typeof(IProductRepository<ProductDataDto>), typeof(ProductRepository));
            services.AddScoped(typeof(IOrderRepository<OrderDataDto>), typeof(OrderRepository.OrderRepository));
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            return services;
        }
    }
}
