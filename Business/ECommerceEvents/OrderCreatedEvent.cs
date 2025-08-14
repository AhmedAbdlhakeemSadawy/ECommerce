using ECommerceBusinessAbstractions;

namespace ECommerceEvents
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public int Id { get; }
        public long OrderNumber { get; }
        public decimal TotalPrice { get; }
        public string CustomerEmail { get; }
        public OrderCreatedEvent(int id, long orderNumber,decimal totalPrice,string customerEmail)
        {
            Id = id;
            OrderNumber = orderNumber;
            TotalPrice = totalPrice;
            CustomerEmail = customerEmail;
        }
    }
}
