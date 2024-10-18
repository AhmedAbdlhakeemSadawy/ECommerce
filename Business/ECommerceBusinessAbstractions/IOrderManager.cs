using ECommerceBuinessDTO;

namespace ECommerceBusinessAbstractions
{
    public interface IOrderManager
    {
        public OrderDTO CreateOrder(CreateOrderDto createOrderDto);
    }
}