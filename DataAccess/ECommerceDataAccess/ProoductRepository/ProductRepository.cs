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

        public IEnumerable<ProductDataDto> UpdateProductsStockQuantity(List<UpdateProductDataStockDto> updateProductDataStockDto)
        {
            // List<ProductDataDto> productDataDtos = new List<ProductDataDto>();
            // List<int> ids = updateProductDataStockDto.Select(p => p.Id).ToList();
            // var products = context.Products.Where(p => ids.Contains(p.Id)).Select(p => new { p.Id, p.StockQuantity }).ToList();


            //for(var i = 0;i < products.Count;i++)
            // {
            //     ProductDataDto productDataDto = new ProductDataDto();
            //     productDataDto.Id = products[i].Id;
            //     products[i].StockQuantity -= updateProductDataStockDto.Where(p => p.Id == updateProductDataStockDto[i].Id).FirstOrDefault().StockQuantity;
            // }

            //context.SaveChanges();

            // return updateProductDataStockDto;

            throw new NotImplementedException();

        }
    }
}
