using AutoMapper;
using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataEntities;
using ECommerceDataAccess.Mapping;
using ECommerceDataAccessAbstraction;
using ECommerceDataAccessDTO;
using Microsoft.EntityFrameworkCore;

namespace ECommerceDataAccess.ProoductRepository
{
    public class ProductRepository : IProductRepository<ProductDataDto>
    {
        private readonly ECommerceDbContext context;
        private IMapper mapper;

        public ProductRepository(ECommerceDbContext context,IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public Task AddAsync(ProductDataDto entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(ProductDataDto entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductDataDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ProductDataDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductDataDto> GetListProductsById(List<int> ids)
        {
            var products = context.Products.AsNoTracking().Where(p => ids.Contains(p.Id)).ToList();
            List<ProductDataDto> productDTOs = products.ToDataDtos();

            return productDTOs;
        }

        public Task UpdateAsync(ProductDataDto entity)
        {
            throw new NotImplementedException();
        }

        public  Task<bool> UpdateProductsStockQuantity(List<ProductDataDto> productDataDtos)
        {
            try
            {
                List<Product> products = productDataDtos.ToEntities();
                context.UpdateRange(products);

                for (int i = 0; i < products.Count; i++)
                {
                    context.Products.Attach(products[i]);
                    context.Entry(products[i]).Property(p => p.StockQuantity).IsModified = true;
                }
                return Task.FromResult(true);
            }
            catch (Exception)
            {
                return Task.FromResult(false);
            }

        }
    }
}
