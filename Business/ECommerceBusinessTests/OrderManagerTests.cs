using ECommerceBuinessDTO;
using ECommerceBusinessLogic;
using ECommerceDataAccess.Abstractions;
using ECommerceDataAccess.DataEntities;
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
            Assert.Equal("Order Should contain al least one prodcut", exception.Message);
        }
    }
}