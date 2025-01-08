using ECommerceBuinessDTO;

namespace ECommerceBusinessAbstractions
{
    public interface IOrderManager
    {
        public OrderBusinessDTO CreateOrder(OrderBusinessDTO createOrderDto);
    }
}