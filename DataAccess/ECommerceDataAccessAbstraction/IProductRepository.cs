using ECommerceCoreContracts;
using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccessAbstraction
{
    public interface IProductRepository<TProduct> : IRepository<TProduct> where TProduct : ProductContract
    {
        IEnumerable<ProductDataDto> GetListProductsById(List<int> ids);
        IEnumerable<ProductDataDto> UpdateProductsStockQuantity(List<ProductDataDto> productDataDtos, List<ProductDataDto> productsDataDtosStockUpdated);
    }
}
