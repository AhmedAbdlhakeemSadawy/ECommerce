using AutoMapper;
using ECommerceBuinessDTO;
using ECommerceBusinessAbstractions;
using ECommerceDataAccessAbstraction;
using ECommerceDataAccessDTO;

namespace ECommerceBusinessLogic
{
    public class OrderManager : IOrderManager
    {
        private IProductRepository<ProductDataDto> productRepository;
        private IOrderRepository<OrderDataDto> orderRepository;
        private IMapper mapper;
        public OrderManager(IProductRepository<ProductDataDto> productRepository, IOrderRepository<OrderDataDto> orderRepository,IMapper mapper)
        {
            this.productRepository = productRepository;
            this.orderRepository = orderRepository;
            this.mapper = mapper;
        }
        public OrderBusinessDTO CreateOrder(OrderBusinessDTO orderBusinessDto)
        {
            
            if (orderBusinessDto.products == null || orderBusinessDto.products.Count == 0)
            {
                throw new Exception("Order Should contain at least one product");
            }
            List<int> ids = orderBusinessDto.products.Select(p => p.Id).ToList();
            var reterivedProdcutsData = productRepository.GetListProductsById(ids).ToList();
            List<ProductBusinessDTO> reterivedProdcutsBusinessDto = mapper.Map<List<ProductBusinessDTO>>(reterivedProdcutsData);


            if (! CheckAvailability(orderBusinessDto.products, reterivedProdcutsData))
            {
                throw new Exception("Some of your products are not available");
            }


            orderBusinessDto.TotalPrice = CalculateOrderTotalPrice(reterivedProdcutsBusinessDto);
            orderBusinessDto.Status = OrderStatus.Created;

            OrderDataDto orderDataDto = mapper.Map<OrderDataDto>(orderBusinessDto);
            var order =   orderRepository.AddOrder(orderDataDto);
            orderBusinessDto.products = UpdateProductsStockQuantities(orderBusinessDto.products, reterivedProdcutsBusinessDto);

            return orderBusinessDto;
    
        }

     
        private bool CheckAvailability(List<ProductBusinessDTO> productBusinessDTOs, List<ProductDataDto> productBusinessDTOsWithSavedQuantities)
        {

            var notAvailableProducts = productBusinessDTOs
                                       .Where(p1 => productBusinessDTOsWithSavedQuantities.Any(p2 => p2.Id == p1.Id && p1.Quantity > p2.StockQuantity))
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

        private decimal CalculateOrderTotalPrice(List<ProductBusinessDTO> productsBusinessDto)
        {
            decimal totalPrice = 0;

            for (int i = 0; i < productsBusinessDto.Count; i++)
            {
                totalPrice += productsBusinessDto[i].Price; 
            }

            return totalPrice;
        }

        private List<ProductBusinessDTO> UpdateProductsStockQuantities(List<ProductBusinessDTO> productsBusinessNeedToUpdateStockDto, List<ProductBusinessDTO> retreviedProductsBusinessStock)
        {
            List<ProductDataDto> productsDataNeedToUpdateStockDto = mapper.Map<List<ProductDataDto>>(productsBusinessNeedToUpdateStockDto);
            List<ProductDataDto> retreviedProductsDataStock = mapper.Map<List<ProductDataDto>>(retreviedProductsBusinessStock);

            List<ProductDataDto> productDataDtosResult = productRepository.UpdateProductsStockQuantity(productsDataNeedToUpdateStockDto, retreviedProductsDataStock).ToList();

            List<ProductBusinessDTO> productsUpdatedResult = mapper.Map<List<ProductBusinessDTO>>(productDataDtosResult);

            return productsUpdatedResult;
        }
    }
}