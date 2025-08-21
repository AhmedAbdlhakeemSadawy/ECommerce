using AutoMapper;
using ECommerceBuinessDTO;
using ECommerceBusinessAbstractions;
using ECommerceDataAccessAbstraction;
using ECommerceDataAccessDTO;
using ECommerceEvents;
using ECommerceInfrastructureAbstraction;

namespace ECommerceBusinessLogic
{
    public class OrderManager : IOrderManager
    {
        private IMapper mapper;
        private IUnitOfWork unitOfWork;
        private IEventBus eventBus;
 
        public OrderManager(IUnitOfWork unitOfWork,IMapper mapper, IEventBus eventBus)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.eventBus = eventBus;
        }
        public async  Task<OrderBusinessDTO> CreateOrder(OrderBusinessDTO orderBusinessDto)
        {
            
            if (orderBusinessDto.products == null || orderBusinessDto.products.Count == 0)
            {
                throw new Exception("Order Should contain at least one product");
            }


            List<int> ids = orderBusinessDto.products.Select(p => p.Id).ToList();
            var reterivedProdcutsData = unitOfWork.ProductRepository.GetListProductsById(ids).ToList();
            List<ProductBusinessDTO> reterivedProdcutsBusinessDto = new List<ProductBusinessDTO>();

            foreach (var productDataDto in reterivedProdcutsData)
            {
                // Find the matching ProductBusinessDTO by ProductId
                var targetProduct = orderBusinessDto.products
                    .FirstOrDefault(p => p.Id == productDataDto.Id);

                if (targetProduct != null)
                {
                    mapper.Map(productDataDto, targetProduct);
                }
            }


            if (! CheckAvailability(orderBusinessDto.products, reterivedProdcutsData))
            {
                throw new Exception("Some of your products are not available");
            }
  


            orderBusinessDto.TotalPrice = CalculateOrderTotalPrice(orderBusinessDto.products);
            orderBusinessDto.Status = OrderStatus.Created;
            orderBusinessDto.OrderNumber = GenerateOrderNumber();

            OrderDataDto orderDataDto = mapper.Map<OrderDataDto>(orderBusinessDto);
            await unitOfWork.OrderRepository.AddOrder(orderDataDto);
            orderBusinessDto.products = UpdateProductsStockQuantities(orderBusinessDto.products);

            await unitOfWork.Complete();

            OrderCreatedEvent orderCreatedEvent = new OrderCreatedEvent(orderBusinessDto.Id,orderBusinessDto.OrderNumber ,orderBusinessDto.TotalPrice,orderBusinessDto.CustomerEmail);

            eventBus.Publish(orderCreatedEvent);
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
                totalPrice += productsBusinessDto[i].Price * productsBusinessDto[i].Quantity; 
            }

            return totalPrice;
        }

        private List<ProductBusinessDTO> UpdateProductsStockQuantities(List<ProductBusinessDTO> productsBusinessNeedToUpdateStockDto)
        {
            List<ProductDataDto> productDataDtosUpdatedStocks = new List<ProductDataDto>();

            mapper.Map(productsBusinessNeedToUpdateStockDto, productDataDtosUpdatedStocks);

            for (int i = 0; i < productsBusinessNeedToUpdateStockDto.Count; i++)
            {
                productDataDtosUpdatedStocks.Where(p => p.Id == productsBusinessNeedToUpdateStockDto[i].Id).FirstOrDefault().StockQuantity = productsBusinessNeedToUpdateStockDto[i].StockQuantity - productsBusinessNeedToUpdateStockDto[i].Quantity;
            }


            List<ProductDataDto> productDataDtosResult = unitOfWork.ProductRepository.UpdateProductsStockQuantity(productDataDtosUpdatedStocks).ToList();

            List<ProductBusinessDTO> productsUpdatedResult = mapper.Map<List<ProductBusinessDTO>>(productDataDtosResult);

            return productsUpdatedResult;
        }

        private  long GenerateOrderNumber()
        {
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long random = new Random().Next(1000); // Add small random component
            return (timestamp + random) % 100000000;
        }
    }
}