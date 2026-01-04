using ECommerceBusinessAbstractions;

namespace ECommerceEvents
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public long OrderNumber { get; }
        public decimal TotalPrice { get; }
        public string CustomerEmail { get; }
        public OrderCreatedEvent(long orderNumber,decimal totalPrice,string customerEmail)
        {
            OrderNumber = orderNumber;
            TotalPrice = totalPrice;
            CustomerEmail = customerEmail;
        }
    }
}
