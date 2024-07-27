using ECommerceDataAccess.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECommerceDataAccess.DatabaseContextConfiguration
{
    public class ECommerceDbContext : DbContext
    {
        public virtual DbSet<Product> Products { get; set; }

   
        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
     : base(options)
        {
        }


        public class ECommerceDbContextFactory : IDesignTimeDbContextFactory<ECommerceDbContext>
        {
            public ECommerceDbContext CreateDbContext(string[] args)
            {
                var optionsBuilder = new DbContextOptionsBuilder<ECommerceDbContext>();
                optionsBuilder.UseSqlServer("Server=.;Initial Catalog=ECommerce;Trusted_Connection=True;Integrated Security=true;Encrypt=False");

                return new ECommerceDbContext(optionsBuilder.Options);
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;Initial Catalog=ECommerce;Trusted_Connection=True;Integrated Security=true;Encrypt=False");
        }


    }
}
