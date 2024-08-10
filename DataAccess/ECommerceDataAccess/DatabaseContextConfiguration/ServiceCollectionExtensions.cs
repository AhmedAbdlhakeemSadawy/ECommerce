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

            return services;
        }
    }
}
