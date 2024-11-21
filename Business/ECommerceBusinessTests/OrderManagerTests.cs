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

            createOrderDto.products.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", StockQuantity = 5 });
            createOrderDto.products.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", StockQuantity = 3 });

            mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(new List<ProductDataDto>
            { new ProductDataDto() { Id = 1 ,StockQuantity = 2} ,new ProductDataDto() { Id = 2 ,StockQuantity = 4}});

            OrderManager orderManager = new OrderManager(mockProductRepository.Object);

            var exception = Assert.Throws<Exception>(() => orderManager.CreateOrder(createOrderDto));
            Assert.Equal("Some of your products are not available", exception.Message);
        }


        [Fact]
        public void CreateOrder_WithAvailableProductQuantity_CalculateTotalPrice()
        {
            var mockProductRepository = new Mock<IProductRepository>();
            CreateOrderDto createOrderDto = new CreateOrderDto();

            createOrderDto.products.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", StockQuantity = 1 });
            createOrderDto.products.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", StockQuantity = 1 });

            mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(new List<ProductDataDto>
            { new ProductDataDto() { Id = 1 ,StockQuantity = 2, Price = 120} ,new ProductDataDto() { Id = 2 ,StockQuantity = 4 , Price = 80}});

         


            OrderManager orderManager = new OrderManager(mockProductRepository.Object);

            OrderDTO orderDTO = orderManager.CreateOrder(createOrderDto);

            Assert.Equal(200, orderDTO.TotalPrice);
        }

        [Fact]
        public void CreateOrder_WithAvailableProductQuantity_UpdateProductStock()
        {
            var mockProductRepository = new Mock<IProductRepository>();
            CreateOrderDto createOrderDto = new CreateOrderDto();

            createOrderDto.products.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", StockQuantity = 1 });
            createOrderDto.products.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", StockQuantity = 1 });

            List <ProductDataDto> retrivedProducts = new List<ProductDataDto>();
            retrivedProducts.Add(new ProductDataDto() { Id = 1, StockQuantity = 2, Price = 120 });
            retrivedProducts.Add(new ProductDataDto() { Id = 2, StockQuantity = 4, Price = 80 });

            mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(retrivedProducts);

            List<ProductDataDto> orderProducts = new List<ProductDataDto>();
            orderProducts.Add(new ProductDataDto() { Id = 1, Name = "Product One", StockQuantity = 1 });
            orderProducts.Add(new ProductDataDto() { Id = 2, Name = "Product Two", StockQuantity = 1 });


            List<ProductDataDto> updatedProducts = new List<ProductDataDto>();
            updatedProducts.Add(new ProductDataDto() { Id = 1, StockQuantity = 1, Price = 120 });
            updatedProducts.Add(new ProductDataDto() { Id = 2, StockQuantity = 3, Price = 80 });

            mockProductRepository.Setup(repo => repo.UpdateProductsStockQuantity(It.IsAny<List<ProductDataDto>>(), It.IsAny<List<ProductDataDto>>())).Returns(updatedProducts);
            OrderManager orderManager = new OrderManager(mockProductRepository.Object);

            OrderDTO orderDTO = orderManager.CreateOrder(createOrderDto);

            mockProductRepository.Verify(repo => repo.UpdateProductsStockQuantity(It.IsAny<List<ProductDataDto>>(), It.IsAny<List<ProductDataDto>>()), Times.Once);

            var actualList = orderDTO.products;
            var expectedList = new List<ProductBusinessDTO> { new ProductBusinessDTO() { Id = 1, StockQuantity = 1}, new ProductBusinessDTO() { Id = 2, StockQuantity = 3 } };

            Assert.Equal(2, orderDTO.products.Count);

            Assert.Equal(1, orderDTO.products[0].Id);
            Assert.Equal(1, orderDTO.products[0].StockQuantity);

            Assert.Equal(2, orderDTO.products[1].Id);
            Assert.Equal(3, orderDTO.products[1].StockQuantity);


        }
    }
}