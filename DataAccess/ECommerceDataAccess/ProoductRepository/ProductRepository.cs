using ECommerceDataAccess.Abstractions;
using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessDTO;
using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<ProductDto> GetListProductsById(List<int> ids)
        {
            var products = context.Products.Where(p => ids.Contains(p.Id)).ToList();
            List<ProductDto> productDTOs = new List<ProductDto>();

            for (var i = 0; i < products.Count; i++)
            {
                ProductDto productDto = new ProductDto();
                productDto.Id = products[i].Id;
                productDto.Name = products[i].name;
                productDto.Description = products[i].description;
                productDto.Price = products[i].price;
                productDto.StockQuantity = products[i].StockQuantity;

                productDTOs.Add(productDto);
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

        public IEnumerable<productStockDTO> UpdateProductsStockQuantity(List<productStockDTO> productStockDTOs)
        {
            List<int> ids = productStockDTOs.Select(p => p.Id).ToList();
            var products = context.Products.Where(p => ids.Contains(p.Id)).Select(p => new { p.Id, p.StockQuantity }).ToList();
            

           for(var i = 0;i < products.Count;i++)
            {
                productStockDTOs[i].StockQuantity -= productStockDTOs.Where(p => p.Id == productStockDTOs[i].Id).FirstOrDefault().StockQuantity;
            }

           context.SaveChanges();

            return productStockDTOs;
        }
    }
}
