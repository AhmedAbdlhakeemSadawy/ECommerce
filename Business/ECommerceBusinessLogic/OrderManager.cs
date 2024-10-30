using ECommerceBuinessDTO;
using ECommerceBusinessAbstractions;

namespace ECommerceBusinessLogic
{
    public class OrderManager : IOrderManager
    {
        public OrderDTO CreateOrder(CreateOrderDto createOrderDto)
        {
            if (createOrderDto.products.Count == 0)
            {
                throw new Exception("Order Should contain al least one prodcut");
            }

            for (int i = 0; i < createOrderDto.products.Count; i++)
            {
                createOrderDto.products[i].q
            }
    
        }
    }
}