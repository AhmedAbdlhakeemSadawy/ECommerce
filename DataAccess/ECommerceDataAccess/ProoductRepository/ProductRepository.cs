using ECommerceDataAccess.Abstractions;
using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccess.ProoductRepository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ECommerceDbContext context;

        public ProductRepository(ECommerceDbContext context)
        {
            this.context = context;
        }
        public Task AddAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public int GetProductStockQuantity(int id)
        {
            return context.Products.Where(p => p.Id == id).FirstOrDefault().StockQuantity;
        }

        public Task UpdateAsync(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
