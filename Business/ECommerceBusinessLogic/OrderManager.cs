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
            var prodcutsData = productRepository.GetListProductsById(ids).ToList();

            //List<ProductDataDto> productsDataDto = new List<ProductDataDto>();

            //for (int i = 0; i < prodcutsWithStock.Count; i++)
            //{
            //    ProductDataDto productDataDto = new ProductDataDto();
            //    productDataDto.Id =  prodcutsWithStock[i].Id;
            //    productDataDto.Price = prodcutsWithStock[i].Price;
            //    productDataDto.StockQuantity = prodcutsWithStock[i].StockQuantity;
            //    productsDataDto.Add(productDataDto);

            //}

            List<ProductBusinessDTO> productReterivedBusinessDTOs = new List<ProductBusinessDTO>();

            for (int i = 0; i < prodcutsData.Count; i++)
            {
                ProductBusinessDTO productBusinessDTO = new ProductBusinessDTO();
                productBusinessDTO.Id = prodcutsData[i].Id;
                productBusinessDTO.Name = prodcutsData[i].Name;
                productBusinessDTO.Price = prodcutsData[i].Price;
                productBusinessDTO.StockQuantity = prodcutsData[i].StockQuantity;
                productReterivedBusinessDTOs.Add(productBusinessDTO);
            }

            if (! CheckAvailability(createOrderDto.products, productReterivedBusinessDTOs))
            {
                throw new Exception("Some of your products are not available");
            }

    

            OrderDTO orderDto = new OrderDTO();
            orderDto.products = UpdateProductsStockQuantities( createOrderDto.products, productReterivedBusinessDTOs);
            orderDto.TotalPrice = CalculateOrderTotalPrice(productReterivedBusinessDTOs);
            
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
            List<ProductDataDto> productsDataDtos = new List<ProductDataDto>();
            for (int i = 0; i < productsDto.Count; i++)
            {
                ProductDataDto productDataDto = new ProductDataDto();
                productDataDto.Id = productsDto[i].Id;
                productDataDto.StockQuantity = productsDto[i].StockQuantity;
                productsDataDtos.Add(productDataDto);

            }

            List<ProductDataDto> productsUpdateStockDataDtos = new List<ProductDataDto>();
            for (int i = 0; i < updateProductDataStockDtos.Count; i++)
            {
                ProductDataDto productDataDto = new ProductDataDto();
                productDataDto.Id = updateProductDataStockDtos[i].Id;
                productDataDto.StockQuantity = updateProductDataStockDtos[i].StockQuantity;
                productsUpdateStockDataDtos.Add(productDataDto);

            }

            List<ProductDataDto> productDataDtosResult = productRepository.UpdateProductsStockQuantity(productsDataDtos, productsUpdateStockDataDtos).ToList();

            List<ProductBusinessDTO> productsUpdatedResult = new List<ProductBusinessDTO>();

            for (int i = 0; i < productDataDtosResult.Count; i++)
            {
                ProductBusinessDTO productBusinessDTO = new ProductBusinessDTO();
                productBusinessDTO.Id = productDataDtosResult[i].Id;
                productBusinessDTO.StockQuantity = productDataDtosResult[i].StockQuantity;
                productsUpdatedResult.Add(productBusinessDTO);

            }
            return productsUpdatedResult;
        }
    }
}