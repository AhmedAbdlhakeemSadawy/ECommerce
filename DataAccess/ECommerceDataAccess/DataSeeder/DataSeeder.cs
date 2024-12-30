using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessAbstraction;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccess.DataSeeder
{
    public  class DataSeeder : IDataSeeder
    {
        private IServiceProvider serviceProvider;

        public DataSeeder(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public void SeedData()
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();


            context.Database.EnsureCreated();
            SeedCustomers(context);
            SeedProducts(context);

        }

        private void SeedCustomers(ECommerceDbContext context)
        {
            if (!context.Customers.Any())
            {
                context.Customers.AddRange(new Customer { email = "customerOne@yahoo.com", name = "Customer One" },
                                           new Customer { email = "customerTwo@yahoo.com", name = "Customer Two" });

                context.SaveChanges();
            }
        }



        private void SeedProducts(ECommerceDbContext context)
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange(new Product {  name = "Product One" ,description = "Description For Product One",price = 20, StockQuantity = 5},
                                           new Product { name = "Product Two", description = "Description For Product Two", price = 10, StockQuantity = 3 },
                                           new Product { name = "Product Three", description = "Description For Product Three", price = 50, StockQuantity = 4 });

                context.SaveChanges();
            }
        }
    }
}
