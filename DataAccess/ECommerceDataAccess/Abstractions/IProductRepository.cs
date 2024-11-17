using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccess.Abstractions
{
    public interface IProductRepository : IRepository<Product>
    {
       // IEnumerable<productStockDTO> GetProductStockQuantity(List<int> ids);

        IEnumerable<ProductDataDto> GetListProductsById(List<int> ids);
        IEnumerable<ProductDataDto> UpdateProductsStockQuantity(List<ProductDataDto> productDataDtos,List<ProductDataDto> productsDataDtosStockUpdated);
    }
}
