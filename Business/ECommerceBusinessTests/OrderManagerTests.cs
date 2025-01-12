using AutoMapper;
using ECommerceBuinessDTO;
using ECommerceBusinessLogic;
using ECommerceDataAccessAbstraction;
using ECommerceDataAccessDTO;
using Moq;
using Xunit;

namespace ECommerceBusinessTests
{
    public class OrderManagerTests
    {
        private Mock<IProductRepository<ProductDataDto>> mockProductRepository;
        private Mock<IOrderRepository<OrderDataDto>> mockOrderRepository;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<IMapper> mockMapper;
        public OrderManagerTests()
        {
             mockProductRepository = new Mock<IProductRepository<ProductDataDto>>();
             mockOrderRepository = new Mock<IOrderRepository<OrderDataDto>>();
             mockUnitOfWork = new Mock<IUnitOfWork>();
             mockMapper = new Mock<IMapper>();


            mockUnitOfWork.Setup(uow => uow.ProductRepository).Returns(mockProductRepository.Object);
            mockUnitOfWork.Setup(uow => uow.OrderRepository).Returns(mockOrderRepository.Object);
        }
        [Fact]
        public void CreateOrder_WithEmptyProducts_ReturnSHouldHaveOneProductException()
        {
            OrderBusinessDTO createOrderDto = new OrderBusinessDTO();

            OrderManager orderManager = new OrderManager(mockUnitOfWork.Object,mockMapper.Object);

            var exception = Assert.Throws<Exception>(() => orderManager.CreateOrder(createOrderDto));
            Assert.Equal("Order Should contain at least one product", exception.Message);
        }

        [Fact]
        public void CreateOrder_WithNotAvailableProductQuantity_ReturnNotAvailableProductsException()
        {

            OrderBusinessDTO createOrderDto = new OrderBusinessDTO();

            List<ProductBusinessDTO> productBusinessDTOs = new List<ProductBusinessDTO>();
            productBusinessDTOs.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", Quantity = 5, Price = 120 });
            productBusinessDTOs.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", Quantity = 3, Price = 80 });

            createOrderDto.products = productBusinessDTOs;

            List<ProductDataDto> retrivedProductDataDtos = new List<ProductDataDto>();
            retrivedProductDataDtos.Add(new ProductDataDto() { Id = 1, StockQuantity = 2  });
            retrivedProductDataDtos.Add(new ProductDataDto() { Id = 2, StockQuantity = 4 });


            mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(retrivedProductDataDtos);


            OrderManager orderManager = new OrderManager(mockUnitOfWork.Object, mockMapper.Object);

            var exception = Assert.Throws<Exception>(() => orderManager.CreateOrder(createOrderDto));
            Assert.Equal("Some of your products are not available", exception.Message);
        }


        [Fact]
        public void CreateOrder_WithAvailableProductQuantity_CalculateTotalPrice()
        {

            OrderBusinessDTO createOrderDto = new OrderBusinessDTO();

            List<ProductBusinessDTO> productBusinessDTOs = new List<ProductBusinessDTO>();
            productBusinessDTOs.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", Quantity = 2, Price = 120 });
            productBusinessDTOs.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", Quantity = 4, Price = 80 });

            createOrderDto.products = productBusinessDTOs;

            List<ProductDataDto> retrivedDataProducts = new List<ProductDataDto>();
            retrivedDataProducts.Add(new ProductDataDto() { Id = 1, StockQuantity = 2 });
            retrivedDataProducts.Add(new ProductDataDto() { Id = 2, StockQuantity = 4 });


            mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(retrivedDataProducts);



            List<ProductBusinessDTO> updatedBusninessProducts = new List<ProductBusinessDTO>();
            updatedBusninessProducts.Add(new ProductBusinessDTO() { Id = 1, Quantity = 1, Price = 120 });
            updatedBusninessProducts.Add(new ProductBusinessDTO() { Id = 2, Quantity = 3, Price = 80 });



            mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(retrivedDataProducts)).Returns(updatedBusninessProducts);

            OrderManager orderManager = new OrderManager(mockUnitOfWork.Object, mockMapper.Object);

            OrderBusinessDTO orderDTO = orderManager.CreateOrder(createOrderDto);

            Assert.Equal(200, orderDTO.TotalPrice);
        }

        [Fact]
        public void CreateOrder_WithAvailableProductQuantity_UpdateProductStock()
        {
            List<ProductBusinessDTO> productsbusinessNeeedToBeUpdated = new List<ProductBusinessDTO>();

            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", Quantity = 1 });
            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", Quantity = 1 });



            List<ProductDataDto> mappedProductsDatasNeeedToBeUpdated = new List<ProductDataDto>();

            mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 1, StockQuantity = 1 });
            mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 2, StockQuantity = 1 });


            OrderBusinessDTO createOrderDto = new OrderBusinessDTO();

            createOrderDto.products = productsbusinessNeeedToBeUpdated;

            List<ProductDataDto> retrivedDataProducts = new List<ProductDataDto>();
            retrivedDataProducts.Add(new ProductDataDto() { Id = 1, StockQuantity = 2 });
            retrivedDataProducts.Add(new ProductDataDto() { Id = 2, StockQuantity = 4 });


            mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(retrivedDataProducts);



            List<ProductDataDto> updatedDataProducts = new List<ProductDataDto>();
            updatedDataProducts.Add(new ProductDataDto() { Id = 1, StockQuantity = 1 });
            updatedDataProducts.Add(new ProductDataDto() { Id = 2, StockQuantity = 3 });


            List<ProductBusinessDTO> updatedBusninessProducts = new List<ProductBusinessDTO>();
            updatedBusninessProducts.Add(new ProductBusinessDTO() { Id = 1, Quantity = 1, Price = 120 });
            updatedBusninessProducts.Add(new ProductBusinessDTO() { Id = 2, Quantity = 3, Price = 80 });



            mockMapper.Setup(map => map.Map<List<ProductDataDto>>(productsbusinessNeeedToBeUpdated)).Returns(mappedProductsDatasNeeedToBeUpdated);
            mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(updatedDataProducts)).Returns(updatedBusninessProducts);
            mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(retrivedDataProducts)).Returns(updatedBusninessProducts);

            mockProductRepository.Setup(repo => repo.UpdateProductsStockQuantity(It.IsAny<List<ProductDataDto>>(), It.IsAny<List<ProductDataDto>>())).Returns(updatedDataProducts);




            OrderManager orderManager = new OrderManager(mockUnitOfWork.Object, mockMapper.Object);

            OrderBusinessDTO orderDTO = orderManager.CreateOrder(createOrderDto);

            mockProductRepository.Verify(repo => repo.UpdateProductsStockQuantity(It.IsAny<List<ProductDataDto>>(), It.IsAny<List<ProductDataDto>>()), Times.Once);


            Assert.Equal(2, orderDTO.products.Count);

            Assert.Equal(1, orderDTO.products[0].Id);
            Assert.Equal(1, orderDTO.products[0].Quantity);

            Assert.Equal(2, orderDTO.products[1].Id);
            Assert.Equal(3, orderDTO.products[1].Quantity);


        }


        [Fact]
        public void CreateOrder_WithValidProducts_SetOrderStatusAsCreated()
        {

            List<ProductBusinessDTO> productsbusinessNeeedToBeUpdated = new List<ProductBusinessDTO>();

            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", Quantity = 1 });
            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", Quantity = 1 });



            List<ProductDataDto> mappedProductsDatasNeeedToBeUpdated = new List<ProductDataDto>();

            mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 1, StockQuantity = 1 });
            mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 2, StockQuantity = 1 });


            OrderBusinessDTO createOrderDto = new OrderBusinessDTO();

            createOrderDto.products = productsbusinessNeeedToBeUpdated;

            List<ProductDataDto> retrivedDataProducts = new List<ProductDataDto>();
            retrivedDataProducts.Add(new ProductDataDto() { Id = 1, StockQuantity = 2 });
            retrivedDataProducts.Add(new ProductDataDto() { Id = 2, StockQuantity = 4 });


            mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(retrivedDataProducts);



            List<ProductDataDto> updatedDataProducts = new List<ProductDataDto>();
            updatedDataProducts.Add(new ProductDataDto() { Id = 1, StockQuantity = 1 });
            updatedDataProducts.Add(new ProductDataDto() { Id = 2, StockQuantity = 3 });


            List<ProductBusinessDTO> updatedBusninessProducts = new List<ProductBusinessDTO>();
            updatedBusninessProducts.Add(new ProductBusinessDTO() { Id = 1, Quantity = 1, Price = 120 });
            updatedBusninessProducts.Add(new ProductBusinessDTO() { Id = 2, Quantity = 3, Price = 80 });



            mockMapper.Setup(map => map.Map<List<ProductDataDto>>(productsbusinessNeeedToBeUpdated)).Returns(mappedProductsDatasNeeedToBeUpdated);
            mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(updatedDataProducts)).Returns(updatedBusninessProducts);
            mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(retrivedDataProducts)).Returns(updatedBusninessProducts);


            mockProductRepository.Setup(repo => repo.UpdateProductsStockQuantity(It.IsAny<List<ProductDataDto>>(), It.IsAny<List<ProductDataDto>>())).Returns(updatedDataProducts);




            OrderManager orderManager = new OrderManager(mockUnitOfWork.Object, mockMapper.Object);

            OrderBusinessDTO orderDTO = orderManager.CreateOrder(createOrderDto);


            Assert.Equal(OrderStatus.Created, orderDTO.Status);

        }



        //[Fact]
        //public void CreateOrder_WithValidProducts_CallAddOrderToSaveInDatabaseSuccess()
        //{
        //    var mockProductRepository = new Mock<IProductRepository<ProductDataDto>>();
        //    var mockOrderRepository = new Mock<IOrderRepository<OrderDataDto>>();

        //    var mockMapper = new Mock<IMapper>();


        //    List<ProductBusinessDTO> productsbusinessNeeedToBeUpdated = new List<ProductBusinessDTO>();

        //    productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", Quantity = 1 });
        //    productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", Quantity = 1 });



        //    List<ProductDataDto> mappedProductsDatasNeeedToBeUpdated = new List<ProductDataDto>();

        //    mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 1, StockQuantity = 1 });
        //    mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 2, StockQuantity = 1 });


        //    CreateOrderDto createOrderDto = new CreateOrderDto();

        //    createOrderDto.products = productsbusinessNeeedToBeUpdated;

        //    List<ProductDataDto> retrivedDataProducts = new List<ProductDataDto>();
        //    retrivedDataProducts.Add(new ProductDataDto() { Id = 1, StockQuantity = 2 });
        //    retrivedDataProducts.Add(new ProductDataDto() { Id = 2, StockQuantity = 4 });


        //    mockProductRepository.Setup(repo => repo.GetListProductsById(new List<int> { 1, 2 })).Returns(retrivedDataProducts);



        //    List<ProductDataDto> updatedDataProducts = new List<ProductDataDto>();
        //    updatedDataProducts.Add(new ProductDataDto() { Id = 1, StockQuantity = 1 });
        //    updatedDataProducts.Add(new ProductDataDto() { Id = 2, StockQuantity = 3 });


        //    List<ProductBusinessDTO> updatedBusninessProducts = new List<ProductBusinessDTO>();
        //    updatedBusninessProducts.Add(new ProductBusinessDTO() { Id = 1, Quantity = 1, Price = 120 });
        //    updatedBusninessProducts.Add(new ProductBusinessDTO() { Id = 2, Quantity = 3, Price = 80 });



        //    mockMapper.Setup(map => map.Map<List<ProductDataDto>>(productsbusinessNeeedToBeUpdated)).Returns(mappedProductsDatasNeeedToBeUpdated);
        //    mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(updatedDataProducts)).Returns(updatedBusninessProducts);

        //    mockProductRepository.Setup(repo => repo.UpdateProductsStockQuantity(It.IsAny<List<ProductDataDto>>(), It.IsAny<List<ProductDataDto>>())).Returns(updatedDataProducts);


        //    mockMapper.Setup(map => map.Map<List<ProductBusinessDTO>>(updatedDataProducts)).Returns(updatedBusninessProducts);


        //    OrderManager orderManager = new OrderManager(mockProductRepository.Object, mockOrderRepository.Object, mockMapper.Object);

        //    OrderBusinessDTO orderDTO = orderManager.CreateOrder(createOrderDto);



        //    Assert.Equal(OrderStatus.Created, orderDTO.Status);

        //}
    }
}