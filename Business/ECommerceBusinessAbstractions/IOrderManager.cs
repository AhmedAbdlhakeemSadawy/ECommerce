using ECommerceBuinessDTO;

namespace ECommerceBusinessAbstractions
{
    public interface IOrderManager
    {
        public Task<OrderBusinessDTO> CreateOrder(OrderBusinessDTO createOrderDto);
        public Task<List<OrderBusinessDTO>> GetAllOrders();
    }
}