using ECommerceBuinessDTO;
using ECommerceBusinessLogic;
using ECommerceDataAccess.Abstractions;
using ECommerceDataAccess.DataEntities;
using ECommerceDataAccessDTO;
using Moq;
using Xunit;

namespace ECommerceBusinessTests
{
    public class OrderManagerTests
    {
        [Fact]
        public void CreateOrder_WithEmptyProducts_ReturnSHouldHaveOneProductException()
        {
            var mockProductRepository = new Mock<IProductRepository>();
            CreateOrderDto createOrderDto = new CreateOrderDto();

            OrderManager orderManager = new OrderManager(mockProductRepository.Object);

            var exception = Assert.Throws<Exception>(() => orderManager.CreateOrder(createOrderDto));
            Assert.Equal("Order Should contain at least one product", exception.Message);
        }

        [Fact]
        public void CreateOrder_WithNotAvailableProductQuantity_ReturnNotAvailableProductsException()
        {
            var mockProductRepository = new Mock<IProductRepository>();
            CreateOrderDto createOrderDto = new CreateOrderDto();

            createOrderDto.products.Add(new ProductDTO { Id = 1, Name = "Product One", Quantiy = 5 });
            createOrderDto.products.Add(new ProductDTO { Id = 2, Name = "Product Two", Quantiy = 3 });

            mockProductRepository.Setup(repo => repo.GetProductStockQuantity(new List<int> { 1, 2 })).Returns(new List<productStockDTO>
            { new productStockDTO() { Id = 1 ,StockQuantity = 2} ,new productStockDTO() { Id = 2 ,StockQuantity = 4}});

            OrderManager orderManager = new OrderManager(mockProductRepository.Object);

            var exception = Assert.Throws<Exception>(() => orderManager.CreateOrder(createOrderDto));
            Assert.Equal("Some of your products are not available", exception.Message);
        }


        [Fact]
        public void CreateOrder_WithAvailableProductQuantity_CalculateTotalPrice()
        {
            var mockProductRepository = new Mock<IProductRepository>();
            CreateOrderDto createOrderDto = new CreateOrderDto();

            createOrderDto.products.Add(new ProductDTO { Id = 1, Name = "Product One", Quantiy = 1 });
            createOrderDto.products.Add(new ProductDTO { Id = 2, Name = "Product Two", Quantiy = 1 });

            mockProductRepository.Setup(repo => repo.GetProductStockQuantity(new List<int> { 1, 2 })).Returns(new List<productStockDTO>
            { new productStockDTO() { Id = 1 ,StockQuantity = 2} ,new productStockDTO() { Id = 2 ,StockQuantity = 4}});


            mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(new List<ECommerceDataAccessDTO.ProductDto>
            { new ECommerceDataAccessDTO.ProductDto() { Id = 1 ,Price = 120} ,new ECommerceDataAccessDTO.ProductDto() { Id = 2 ,Price = 80}});

            OrderManager orderManager = new OrderManager(mockProductRepository.Object);

            OrderDTO orderDTO = orderManager.CreateOrder(createOrderDto);

            Assert.Equal(200, orderDTO.TotalPrice);
        }
    }
}