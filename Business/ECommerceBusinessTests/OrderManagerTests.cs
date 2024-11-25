using AutoMapper;
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
            var mockMapper = new Mock<IMapper>();

            CreateOrderDto createOrderDto = new CreateOrderDto();

            OrderManager orderManager = new OrderManager(mockProductRepository.Object, mockMapper.Object);

            var exception = Assert.Throws<Exception>(() => orderManager.CreateOrder(createOrderDto));
            Assert.Equal("Order Should contain at least one product", exception.Message);
        }

        [Fact]
        public void CreateOrder_WithNotAvailableProductQuantity_ReturnNotAvailableProductsException()
        {
            var mockProductRepository = new Mock<IProductRepository>();
            var mockMapper = new Mock<IMapper>();

            CreateOrderDto createOrderDto = new CreateOrderDto();

            List<ProductBusinessDTO> productBusinessDTOs = new List<ProductBusinessDTO>();
            productBusinessDTOs.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", StockQuantity = 5, Price = 120 });
            productBusinessDTOs.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", StockQuantity = 3, Price = 80 });

            createOrderDto.products = productBusinessDTOs;

            List<ProductDataDto> productDataDtos = new List<ProductDataDto>();
            productDataDtos.Add(new ProductDataDto() { Id = 1, StockQuantity = 2, Price = 120 });
            productDataDtos.Add(new ProductDataDto() { Id = 2, StockQuantity = 4, Price = 80 });

            List<ProductBusinessDTO> retreivedProductsBusinessDtos = new List<ProductBusinessDTO>();
            retreivedProductsBusinessDtos.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", StockQuantity = 2, Price = 120 });
            retreivedProductsBusinessDtos.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", StockQuantity = 4, Price = 80 });

            mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(productDataDtos);

            mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(productDataDtos)).Returns(retreivedProductsBusinessDtos);


            OrderManager orderManager = new OrderManager(mockProductRepository.Object, mockMapper.Object);

            var exception = Assert.Throws<Exception>(() => orderManager.CreateOrder(createOrderDto));
            Assert.Equal("Some of your products are not available", exception.Message);
        }


        [Fact]
        public void CreateOrder_WithAvailableProductQuantity_CalculateTotalPrice()
        {
            var mockProductRepository = new Mock<IProductRepository>();
            var mockMapper = new Mock<IMapper>();
            CreateOrderDto createOrderDto = new CreateOrderDto();

            List<ProductBusinessDTO> productBusinessDTOs = new List<ProductBusinessDTO>();
            productBusinessDTOs.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", StockQuantity = 2, Price = 120 });
            productBusinessDTOs.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", StockQuantity = 4, Price = 80 });

            createOrderDto.products = productBusinessDTOs;

            List<ProductDataDto> productDataDtos = new List<ProductDataDto>();
            productDataDtos.Add(new ProductDataDto() { Id = 1, StockQuantity = 2, Price = 120 });
            productDataDtos.Add(new ProductDataDto() { Id = 2, StockQuantity = 4, Price = 80 });


            mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(productDataDtos);

            mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(productDataDtos)).Returns(productBusinessDTOs);


            OrderManager orderManager = new OrderManager(mockProductRepository.Object,mockMapper.Object);

            OrderDTO orderDTO = orderManager.CreateOrder(createOrderDto);

            Assert.Equal(200, orderDTO.TotalPrice);
        }

        [Fact]
        public void CreateOrder_WithAvailableProductQuantity_UpdateProductStock()
        {
            var mockProductRepository = new Mock<IProductRepository>();
            var mockMapper = new Mock<IMapper>();


            List<ProductDataDto> productsDataNeeedToBeUpdated = new List<ProductDataDto>();

            productsDataNeeedToBeUpdated.Add(new ProductDataDto { Id = 1, Name = "Product One", StockQuantity = 1 });
            productsDataNeeedToBeUpdated.Add(new ProductDataDto { Id = 2, Name = "Product Two", StockQuantity = 1 });


            List<ProductBusinessDTO> productsbusinessNeeedToBeUpdated = new List<ProductBusinessDTO>();

            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", StockQuantity = 1 });
            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", StockQuantity = 1 });


            CreateOrderDto createOrderDto = new CreateOrderDto();

            createOrderDto.products = productsbusinessNeeedToBeUpdated;

            List <ProductDataDto> retrivedDataProducts = new List<ProductDataDto>();
            retrivedDataProducts.Add(new ProductDataDto() { Id = 1, StockQuantity = 2, Price = 120 });
            retrivedDataProducts.Add(new ProductDataDto() { Id = 2, StockQuantity = 4, Price = 80 });

            List<ProductBusinessDTO> retrivedBusinessProducts = new List<ProductBusinessDTO>();
            retrivedBusinessProducts.Add(new ProductBusinessDTO() { Id = 1, StockQuantity = 2, Price = 120 });
            retrivedBusinessProducts.Add(new ProductBusinessDTO() { Id = 2, StockQuantity = 4, Price = 80 });

            mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(retrivedDataProducts);

            List<ProductDataDto> orderProducts = new List<ProductDataDto>();
            orderProducts.Add(new ProductDataDto() { Id = 1, Name = "Product One", StockQuantity = 1 });
            orderProducts.Add(new ProductDataDto() { Id = 2, Name = "Product Two", StockQuantity = 1 });


            List<ProductBusinessDTO> orderBusniessProducts = new List<ProductBusinessDTO>();
            orderBusniessProducts.Add(new ProductBusinessDTO() { Id = 1, Name = "Product One", StockQuantity = 1 });
            orderBusniessProducts.Add(new ProductBusinessDTO() { Id = 2, Name = "Product Two", StockQuantity = 1 });


            List<ProductDataDto> updatedProducts = new List<ProductDataDto>();
            updatedProducts.Add(new ProductDataDto() { Id = 1, StockQuantity = 1, Price = 120 });
            updatedProducts.Add(new ProductDataDto() { Id = 2, StockQuantity = 3, Price = 80 });


            List<ProductBusinessDTO> updatedBusninessProducts = new List<ProductBusinessDTO>();
            updatedBusninessProducts.Add(new ProductBusinessDTO() { Id = 1, StockQuantity = 1, Price = 120 });
            updatedBusninessProducts.Add(new ProductBusinessDTO() { Id = 2, StockQuantity = 3, Price = 80 });

            mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(productsDataNeeedToBeUpdated)).Returns(productsbusinessNeeedToBeUpdated);
            mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(retrivedDataProducts)).Returns(retrivedBusinessProducts);

            mockMapper.Setup(map => map.Map<List<ProductDataDto>>(productsbusinessNeeedToBeUpdated)).Returns(productsDataNeeedToBeUpdated);
            mockMapper.Setup(map => map.Map<List<ProductDataDto>>(retrivedBusinessProducts)).Returns(retrivedDataProducts);


            mockProductRepository.Setup(repo => repo.UpdateProductsStockQuantity(It.IsAny<List<ProductDataDto>>(), It.IsAny<List<ProductDataDto>>())).Returns(updatedProducts);

            mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(updatedProducts)).Returns(updatedBusninessProducts);


            OrderManager orderManager = new OrderManager(mockProductRepository.Object, mockMapper.Object);

            OrderDTO orderDTO = orderManager.CreateOrder(createOrderDto);

            mockProductRepository.Verify(repo => repo.UpdateProductsStockQuantity(It.IsAny<List<ProductDataDto>>(), It.IsAny<List<ProductDataDto>>()), Times.Once);

        
            Assert.Equal(2, orderDTO.products.Count);

            Assert.Equal(1, orderDTO.products[0].Id);
            Assert.Equal(1, orderDTO.products[0].StockQuantity);

            Assert.Equal(2, orderDTO.products[1].Id);
            Assert.Equal(3, orderDTO.products[1].StockQuantity);


        }


        [Fact]
        public void CreateOrder_WithValidProducts_SetOrderStatusAsCreated()
        {
            var mockProductRepository = new Mock<IProductRepository>();
            var mockMapper = new Mock<IMapper>();
            CreateOrderDto createOrderDto = new CreateOrderDto();

            List<ProductBusinessDTO> productBusinessDTOs = new List<ProductBusinessDTO>();
            productBusinessDTOs.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", StockQuantity = 2, Price = 120 });
            productBusinessDTOs.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", StockQuantity = 4, Price = 80 });

            createOrderDto.products = productBusinessDTOs;

            List<ProductDataDto> productDataDtos = new List<ProductDataDto>();
            productDataDtos.Add(new ProductDataDto() { Id = 1, StockQuantity = 2, Price = 120 });
            productDataDtos.Add(new ProductDataDto() { Id = 2, StockQuantity = 4, Price = 80 });


            mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(productDataDtos);

            mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(productDataDtos)).Returns(productBusinessDTOs);


            OrderManager orderManager = new OrderManager(mockProductRepository.Object, mockMapper.Object);

            OrderDTO orderDTO = orderManager.CreateOrder(createOrderDto);

            Assert.Equal(OrderStatus.Created, orderDTO.Status);

        }
    }
}