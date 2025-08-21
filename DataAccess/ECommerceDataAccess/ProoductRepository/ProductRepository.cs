using AutoMapper;
using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataEntities;
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

        public IEnumerable<ProductDataDto> UpdateProductsStockQuantity(List<ProductDataDto> productDataDtos)
        {
            List<ProductDataDto> productsUpdated = new List<ProductDataDto>();

            List<Product> products = mapper.Map<List<Product>>(productDataDtos);
            context.UpdateRange(products);

            for (int i = 0; i < products.Count; i++)
            {
                context.Products.Attach(products[i]);
                context.Entry(products[i]).Property(p => p.StockQuantity).IsModified = true;
            }

            return productDataDtos;
        }
    }
}
