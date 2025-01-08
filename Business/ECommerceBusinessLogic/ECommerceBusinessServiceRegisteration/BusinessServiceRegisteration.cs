using ECommerceBusinessAbstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceBusinessLogic.ECommerceBusinessServiceRegisteration
{
    public static class BusinessServiceRegisteration
    {
        public static IServiceCollection RegisterBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IOrderManager, OrderManager>();
            return services;
        }
    }
}
