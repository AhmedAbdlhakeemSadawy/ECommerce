using ECommerceBuinessDTO;
using ECommerceBusinessAbstractions;
using ECommerceDataAccess.Abstractions;

namespace ECommerceBusinessLogic
{
    public class OrderManager : IOrderManager
    {
        private IProductRepository productRepository;
        public OrderManager(IProductRepository productRepository)
        {
                this.productRepository = productRepository;
        }
        public OrderDTO CreateOrder(CreateOrderDto createOrderDto)
        {
            if (createOrderDto.products.Count == 0)
            {
                throw new Exception("Order Should contain al least one prodcut");
            }

            if (! CheckAvailability(createOrderDto.products))
            {
                throw new Exception("Some of your products are not available");
            }
         
    
        }

        private bool CheckAvailability(List<ProductDTO> productsDto)
        {
            List<int> ids = productsDto.Select(p => p.Id).ToList();
            var prodcutsWithStock = productRepository.GetListProductsById(ids).ToList();

            var notAvailableProducts = productsDto
                                       .Where(p1 => prodcutsWithStock.Any(p2 => p2.Id == p1.Id && p1.Quantiy > p2.StockQuantity))
                                       .ToList();

            if (notAvailableProducts.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}