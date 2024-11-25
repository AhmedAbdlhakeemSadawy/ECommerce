using AutoMapper;
using ECommerceBuinessDTO;
using ECommerceBusinessAbstractions;
using ECommerceDataAccess.Abstractions;
using ECommerceDataAccessDTO;

namespace ECommerceBusinessLogic
{
    public class OrderManager : IOrderManager
    {
        private IProductRepository productRepository;
        private IMapper mapper;
        public OrderManager(IProductRepository productRepository, IMapper mapper)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
        }
        public OrderDTO CreateOrder(CreateOrderDto createOrderDto)
        {
            
            if (createOrderDto.products == null || createOrderDto.products.Count == 0)
            {
                throw new Exception("Order Should contain at least one product");
            }
            List<int> ids = createOrderDto.products.Select(p => p.Id).ToList();
            var prodcutsData = productRepository.GetListProductsById(ids).ToList();


            List<ProductBusinessDTO> productReterivedBusinessDTOs = mapper.Map<List<ProductBusinessDTO>>(prodcutsData);


            if (! CheckAvailability(createOrderDto.products, productReterivedBusinessDTOs))
            {
                throw new Exception("Some of your products are not available");
            }

            OrderDTO orderDto = new OrderDTO();
            orderDto.products = UpdateProductsStockQuantities( createOrderDto.products, productReterivedBusinessDTOs);
            orderDto.TotalPrice = CalculateOrderTotalPrice(productReterivedBusinessDTOs);
            orderDto.Status = OrderStatus.Created;
            return orderDto;
    
        }

     
        private bool CheckAvailability(List<ProductBusinessDTO> productBusinessDTOs, List<ProductBusinessDTO> productBusinessDTOsWithSavedQuantities)
        {

            var notAvailableProducts = productBusinessDTOs
                                       .Where(p1 => productBusinessDTOsWithSavedQuantities.Any(p2 => p2.Id == p1.Id && p1.StockQuantity > p2.StockQuantity))
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

        private List<ProductBusinessDTO> UpdateProductsStockQuantities(List<ProductBusinessDTO> productsDto, List<ProductBusinessDTO> updateProductDataStockDtos)
        {
            List<ProductDataDto> productsDataDtos = mapper.Map<List<ProductDataDto>>(productsDto);
            List<ProductDataDto> productsUpdateStockDataDtos = mapper.Map<List<ProductDataDto>>(updateProductDataStockDtos);

            List<ProductDataDto> productDataDtosResult = productRepository.UpdateProductsStockQuantity(productsDataDtos, productsUpdateStockDataDtos).ToList();

            List<ProductBusinessDTO> productsUpdatedResult = mapper.Map<List<ProductBusinessDTO>>(productDataDtosResult);

            return productsUpdatedResult;
        }
    }
}