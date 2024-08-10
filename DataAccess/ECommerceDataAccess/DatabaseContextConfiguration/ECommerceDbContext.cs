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

        public ECommerceDbContext(DbContextOptions<ECommerceDbContext> options)
     : base(options)
        {
        }
        public virtual DbSet<Product> Products { get; set; }


    }
}
