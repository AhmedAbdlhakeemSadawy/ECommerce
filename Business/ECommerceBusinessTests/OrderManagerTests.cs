using AutoMapper;
using ECommerceBuinessDTO;
using ECommerceBusinessLogic;
using ECommerceDataAccessAbstraction;
using ECommerceDataAccessDTO;
using ECommerceEvents;
using ECommerceInfrastructureAbstraction;
using Moq;
using Xunit;

namespace ECommerceBusinessTests
{
    public class OrderManagerTests
    {
        private Mock<IProductRepository<ProductDataDto>> mockProductRepository;
        private Mock<IOrderRepository<OrderDataDto>> mockOrderRepository;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<IEventBus> mockEventBus;
        private Mock<IMapper> mockMapper;
        public OrderManagerTests()
        {
             mockProductRepository = new Mock<IProductRepository<ProductDataDto>>();
             mockOrderRepository = new Mock<IOrderRepository<OrderDataDto>>();
             mockUnitOfWork = new Mock<IUnitOfWork>();
             mockMapper = new Mock<IMapper>();
             mockEventBus = new Mock<IEventBus>();


            mockUnitOfWork.Setup(uow => uow.ProductRepository).Returns(mockProductRepository.Object);
            mockUnitOfWork.Setup(uow => uow.OrderRepository).Returns(mockOrderRepository.Object);
        }
        [Fact]
        public async Task CreateOrder_WithEmptyProducts_ReturnSHouldHaveOneProductException()
        {
            OrderBusinessDTO createOrderDto = new OrderBusinessDTO();

            OrderManager orderManager = new OrderManager(mockUnitOfWork.Object,mockMapper.Object, mockEventBus.Object);


            var exception = await Assert.ThrowsAsync<BusinessException>(async () => await orderManager.CreateOrder(createOrderDto));
            Assert.Equal("Order Should contain at least one product", exception.Message);
        }

        [Fact]
        public async Task CreateOrder_WithNotAvailableProductQuantity_ReturnNotAvailableProductsException()
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


            OrderManager orderManager = new OrderManager(mockUnitOfWork.Object, mockMapper.Object, mockEventBus.Object);

            var exception = await Assert.ThrowsAsync<BusinessException>(async () => await orderManager.CreateOrder(createOrderDto));
            Assert.Equal("Some of your products are not available", exception.Message);
        }


        [Fact]
        public async Task CreateOrder_WithAvailableProductQuantity_CalculateTotalPrice()
        {

            OrderBusinessDTO createOrderDto = new OrderBusinessDTO();

            List<ProductBusinessDTO> productBusinessDTOs = new List<ProductBusinessDTO>();
            productBusinessDTOs.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", Quantity = 2, Price = 20 });
            productBusinessDTOs.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", Quantity = 4, Price = 10 });

            createOrderDto.products = productBusinessDTOs;

            OrderManager orderManager = new OrderManager(mockUnitOfWork.Object, mockMapper.Object, mockEventBus.Object);

            OrderBusinessDTO orderDTO = await orderManager.CreateOrder(createOrderDto);

            Assert.Equal(80, orderDTO.TotalPrice);
        }

        [Fact]
        public async Task CreateOrder_WithAvailableProductQuantity_UpdateProductStock()
        {
            List<ProductBusinessDTO> productsbusinessNeeedToBeUpdated = new List<ProductBusinessDTO>();

            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", Quantity = 2 ,StockQuantity = 5 });
            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", Quantity = 1 , StockQuantity = 3});



            List<ProductDataDto> mappedProductsDatasNeeedToBeUpdated = new List<ProductDataDto>();

            mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 1, StockQuantity = 5 });
            mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 2, StockQuantity = 3 });


            OrderBusinessDTO createOrderDto = new OrderBusinessDTO();

            createOrderDto.products = productsbusinessNeeedToBeUpdated;

            mockMapper.Setup(map => map.Map<List<ProductDataDto>>(createOrderDto.products)).Returns(mappedProductsDatasNeeedToBeUpdated);

            mockProductRepository.Setup(repo => repo.UpdateProductsStockQuantity(It.IsAny<List<ProductDataDto>>())).Returns(Task.FromResult(true));




            OrderManager orderManager = new OrderManager(mockUnitOfWork.Object, mockMapper.Object, mockEventBus.Object);

            OrderBusinessDTO orderDTO = await orderManager.CreateOrder(createOrderDto);

            mockProductRepository.Verify(repo => repo.UpdateProductsStockQuantity(It.IsAny<List<ProductDataDto>>()), Times.Once);



            Assert.Equal(2, orderDTO.products.Count);
            Assert.Equal(3, orderDTO.products[0].StockQuantity);
            Assert.Equal(2, orderDTO.products[1].StockQuantity);

        }


        [Fact]
        public async Task CreateOrder_WithValidProducts_SetOrderStatusAsCreated()
        {

            List<ProductBusinessDTO> productsbusinessNeeedToBeUpdated = new List<ProductBusinessDTO>();

            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", Quantity = 1 });
            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", Quantity = 1 });



            List<ProductDataDto> mappedProductsDatasNeeedToBeUpdated = new List<ProductDataDto>();

            mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 1, StockQuantity = 1 });
            mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 2, StockQuantity = 1 });


            OrderBusinessDTO createOrderDto = new OrderBusinessDTO();

            createOrderDto.products = productsbusinessNeeedToBeUpdated;



            OrderManager orderManager = new OrderManager(mockUnitOfWork.Object, mockMapper.Object,mockEventBus.Object);

            OrderBusinessDTO orderDTO = await orderManager.CreateOrder(createOrderDto);


            Assert.Equal(OrderStatus.Created, orderDTO.Status);

        }



        [Fact]
        public async Task CreateOrder_WithValidProducts_CallAddOrderToSaveInDatabaseSuccess()
        {
            List<ProductBusinessDTO> productsbusinessNeeedToBeUpdated = new List<ProductBusinessDTO>();

            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", Quantity = 2, StockQuantity = 5 });
            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", Quantity = 1, StockQuantity = 3 });



            List<ProductDataDto> mappedProductsDatasNeeedToBeUpdated = new List<ProductDataDto>();

            mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 1, StockQuantity = 5 });
            mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 2, StockQuantity = 3 });


            OrderBusinessDTO createOrderDto = new OrderBusinessDTO();

            createOrderDto.products = productsbusinessNeeedToBeUpdated;

            mockMapper.Setup(map => map.Map<OrderDataDto>(createOrderDto)).Returns(new OrderDataDto());




            OrderManager orderManager = new OrderManager(mockUnitOfWork.Object, mockMapper.Object, mockEventBus.Object);

            OrderBusinessDTO orderDTO = await orderManager.CreateOrder(createOrderDto);

            mockOrderRepository.Verify(repo => repo.AddOrder(It.IsAny<OrderDataDto>()), Times.Once);



        }


        [Fact]
        public async Task CreateOrder_WithValidProducts_OrderCreatedEventAdded()
        {
            List<ProductBusinessDTO> productsbusinessNeeedToBeUpdated = new List<ProductBusinessDTO>();

            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 1, Name = "Product One", Quantity = 2, StockQuantity = 5 });
            productsbusinessNeeedToBeUpdated.Add(new ProductBusinessDTO { Id = 2, Name = "Product Two", Quantity = 1, StockQuantity = 3 });



            List<ProductDataDto> mappedProductsDatasNeeedToBeUpdated = new List<ProductDataDto>();

            mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 1, StockQuantity = 5 });
            mappedProductsDatasNeeedToBeUpdated.Add(new ProductDataDto { Id = 2, StockQuantity = 3 });


            OrderBusinessDTO createOrderDto = new OrderBusinessDTO();

            createOrderDto.products = productsbusinessNeeedToBeUpdated;

            mockMapper.Setup(map => map.Map<OrderDataDto>(createOrderDto)).Returns(new OrderDataDto());




            OrderManager orderManager = new OrderManager(mockUnitOfWork.Object, mockMapper.Object, mockEventBus.Object);

            OrderBusinessDTO orderDTO = await orderManager.CreateOrder(createOrderDto);

            mockEventBus.Verify(eventBus => eventBus.Publish(It.IsAny<OrderCreatedEvent>()), Times.Once);



        }
    }
}