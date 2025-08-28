using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccessAbstraction
{
    public interface IProductRepository<TProductDto> : IRepository<TProductDto> where TProductDto : ProductDataDto
    {
        IEnumerable<ProductDataDto> GetListProductsById(List<int> ids);
        Task<bool> UpdateProductsStockQuantity(List<ProductDataDto> productDataDtos);
    }
}
