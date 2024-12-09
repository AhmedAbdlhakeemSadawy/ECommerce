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
        public OrderBusinessDTO CreateOrder(CreateOrderDto createOrderDto)
        {
            
            if (createOrderDto.products == null || createOrderDto.products.Count == 0)
            {
                throw new Exception("Order Should contain at least one product");
            }
            List<int> ids = createOrderDto.products.Select(p => p.Id).ToList();
            var reterivedProdcutsData = productRepository.GetListProductsById(ids).ToList();


            if (! CheckAvailability(createOrderDto.products, reterivedProdcutsData))
            {
                throw new Exception("Some of your products are not available");
            }

            OrderBusinessDTO orderBusinessDto = new OrderBusinessDTO();
            orderBusinessDto.products = UpdateProductsStockQuantities( createOrderDto.products, reterivedProdcutsData);
            orderBusinessDto.TotalPrice = CalculateOrderTotalPrice(createOrderDto.products);
            orderBusinessDto.Status = OrderStatus.Created;

            OrderDataDto orderDataDto = mapper.Map<OrderDataDto>(orderBusinessDto);
            var order =   orderRepository.AddAsync(orderDataDto);
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

        private List<ProductBusinessDTO> UpdateProductsStockQuantities(List<ProductBusinessDTO> productsBusinessNeedToUpdateStockDto, List<ProductDataDto> retreviedProductsDataStock)
        {
            List<ProductDataDto> productsDataNeedToUpdateStockDto = mapper.Map<List<ProductDataDto>>(productsBusinessNeedToUpdateStockDto);
           // List<ProductDataDto> productsUpdateStockDataDtos = mapper.Map<List<ProductDataDto>>(updateProductDataStockDtos);

            List<ProductDataDto> productDataDtosResult = productRepository.UpdateProductsStockQuantity(productsDataNeedToUpdateStockDto, retreviedProductsDataStock).ToList();

            List<ProductBusinessDTO> productsUpdatedResult = mapper.Map<List<ProductBusinessDTO>>(productDataDtosResult);

            return productsUpdatedResult;
        }
    }
}