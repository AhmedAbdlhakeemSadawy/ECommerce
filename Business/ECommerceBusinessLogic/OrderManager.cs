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
            if (createOrderDto.products == null || createOrderDto.products.Count == 0)
            {
                throw new Exception("Order Should contain at least one product");
            }

            if (! CheckAvailability(createOrderDto.products))
            {
                throw new Exception("Some of your products are not available");
            }


            OrderDTO orderDto = new OrderDTO();
            orderDto.products = createOrderDto.products;
            orderDto.TotalPrice = CalculateOrderTotalPrice(createOrderDto.products);
            
            return orderDto;
    
        }

     
        private bool CheckAvailability(List<ProductDTO> productsDto)
        {
            List<int> ids = productsDto.Select(p => p.Id).ToList();
            var prodcutsWithStock = productRepository.GetProductStockQuantity(ids).ToList();

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

        private decimal CalculateOrderTotalPrice(List<ProductDTO> productsDto)
        {
            decimal totalPrice = 0;
            List<int> ids = productsDto.Select(p => p.Id).ToList();

            var prodcuts = productRepository.GetListProductsById(ids).ToList();

            for (int i = 0; i < prodcuts.Count; i++)
            {
                totalPrice += prodcuts[i].Price; 
            }

            return totalPrice;
        }
    }
}