using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessAbstraction;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ECommerceDataAccess.DataSeeder
{
    public  class DataSeeder : IDataSeeder
    {
        private IServiceProvider serviceProvider;

        public DataSeeder(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public async Task SeedData()
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ECommerceDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            context.Database.EnsureCreated();
            SeedCustomers(context);
            SeedProducts(context);
            await SeedRolesAsync(roleManager);

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

        private async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
