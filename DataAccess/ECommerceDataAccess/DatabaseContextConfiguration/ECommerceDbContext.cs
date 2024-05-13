using ECommerceDataAccess.DataEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ECommerceDataAccess.DatabaseContextConfiguration
{
    internal class ECommerceDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Here you would configure your database connection, for example:
                optionsBuilder.UseSqlServer("YourConnectionString");
            }
        }
    }
}
