using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessDTO;
using ECommerceDataAccessAbstraction;
using Microsoft.EntityFrameworkCore;

namespace ECommerceDataAccess.ProoductRepository
{
    public class ProductRepository : IProductRepository<ProductDataDto>
    {
        private readonly ECommerceDbContext context;

        public ProductRepository(ECommerceDbContext context)
        {
            this.context = context;
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
            List<ProductDataDto> productDTOs = new List<ProductDataDto>();

            for (var i = 0; i < products.Count; i++)
            {
                ProductDataDto productDataDto = new ProductDataDto();
                productDataDto.Id = products[i].Id;
                productDataDto.price = products[i].price;
                productDataDto.StockQuantity = products[i].StockQuantity;

                productDTOs.Add(productDataDto);
            }

            return productDTOs;
        }

        public Task UpdateAsync(ProductDataDto entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductDataDto> UpdateProductsStockQuantity(List<ProductDataDto> productDataDtos, List<ProductDataDto> productsDataDtosStockUpdated)
        {
            List<ProductDataDto> productsUpdated = new List<ProductDataDto>();

            for (int i = 0; i < productDataDtos.Count; i++)
            {
                Product product = new Product();
                product.Id = productDataDtos[i].Id;
                product.StockQuantity = productsDataDtosStockUpdated.Where(p => p.Id == productsDataDtosStockUpdated[i].Id).FirstOrDefault().StockQuantity - productDataDtos[i].StockQuantity;
                context.Entry(product).Property(p => p.StockQuantity).IsModified = true;
                context.Products.Attach(product);
                productsUpdated.Add(new ProductDataDto { Id = productDataDtos[i].Id, StockQuantity = product.StockQuantity });
            }

            context.SaveChanges();

            return productsUpdated;
        }
    }
}
