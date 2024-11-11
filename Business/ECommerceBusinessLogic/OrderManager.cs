using ECommerceBuinessDTO;
using ECommerceBusinessAbstractions;
using ECommerceDataAccess.Abstractions;
using ECommerceDataAccessDTO;

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
            List<int> ids = createOrderDto.products.Select(p => p.Id).ToList();
            var prodcutsWithStock = productRepository.GetListProductsById(ids).ToList();

            List<ProductDataDto> productsDataDto = new List<ProductDataDto>();
             for (int i = 0; i < prodcutsWithStock.Count; i++)
            {
                ProductDataDto productDataDto = new ProductDataDto();
                productDataDto.Id = prodcutsWithStock[i].Id;
                productDataDto.Price = prodcutsWithStock[i].Price;
                productDataDto.StockQuantity = prodcutsWithStock[i].StockQuantity;

                productsDataDto.Add(productDataDto);
            }

            if (! CheckAvailability(createOrderDto.products, productsDataDto))
            {
                throw new Exception("Some of your products are not available");
            }


            OrderDTO orderDto = new OrderDTO();
            orderDto.products = createOrderDto.products;
            orderDto.TotalPrice = CalculateOrderTotalPrice(productsDataDto);
            
            return orderDto;
    
        }

     
        private bool CheckAvailability(List<ProductDTO> productsDto, List<ProductDataDto> productsDataDto)
        {

            var notAvailableProducts = productsDto
                                       .Where(p1 => productsDataDto.Any(p2 => p2.Id == p1.Id && p1.Quantiy > p2.StockQuantity))
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

        private decimal CalculateOrderTotalPrice(List<ProductDataDto> productsDataDto)
        {
            decimal totalPrice = 0;

            for (int i = 0; i < productsDataDto.Count; i++)
            {
                totalPrice += productsDataDto[i].Price; 
            }

            return totalPrice;
        }
    }
}