using ECommerceDataAccess.Abstractions;
using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessDTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        public IEnumerable<ProductDataDto> GetListProductsById(List<int> ids)
        {
            var products = context.Products.Where(p => ids.Contains(p.Id)).ToList();
            List<ProductDataDto> productDTOs = new List<ProductDataDto>();

            for (var i = 0; i < products.Count; i++)
            {
                ProductDataDto productDataDto = new ProductDataDto();
                productDataDto.Id = products[i].Id;
                productDataDto.Price = products[i].price;
                productDataDto.StockQuantity = products[i].StockQuantity;

                productDTOs.Add(productDataDto);
            }

            return productDTOs;
        }

        public IEnumerable<productStockDTO> GetProductStockQuantity(List<int> ids)
        {
            var products = context.Products.Where(p => ids.Contains(p.Id)).Select(p => new { p.Id, p.StockQuantity }).ToList();
            List<productStockDTO> productStockDTOs = new List<productStockDTO>();

            for (var i = 0; i < products.Count; i++)
            {
                productStockDTO productStockDTO = new productStockDTO();
                productStockDTO.Id = products[i].Id;
                productStockDTO.StockQuantity = products[i].StockQuantity;

                productStockDTOs.Add(productStockDTO);
            }

            return productStockDTOs;
           
        }

        public Task UpdateAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ProductDataDto> UpdateProductsStockQuantity(List<ProductDataDto> productDataDtos, List<ProductDataDto> updateProductDataStockDtos)
        {
            List<ProductDataDto> productsUpdated = new List<ProductDataDto>();


            for (int i = 0; i < productDataDtos.Count; i++)
            {
                Product product = new Product();
                product.Id = productDataDtos[i].Id;
                product.StockQuantity = updateProductDataStockDtos.Where(p => p.Id == updateProductDataStockDtos[i].Id).FirstOrDefault().StockQuantity - productDataDtos[i].StockQuantity;
                context.Entry(product).Property(p=> p.StockQuantity).IsModified = true;
                context.Products.Attach(product);
                productsUpdated.Add(new ProductDataDto { Id = productDataDtos[i].Id, StockQuantity = product.StockQuantity });
            }


            context.SaveChanges();


            return productsUpdated;


        }
    }
}
