using ECommerceBusinessAbstractions;

namespace ECommerceEvents
{
    public class OrderCreatedEvent : IDomainEvent
    {
        public int Id { get; }
        public decimal TotalPrice { get; }
        public string CustomerEmail { get; }
        public OrderCreatedEvent(int id, decimal totalPrice,string customerEmail)
        {
            Id = id;
            TotalPrice = totalPrice;
            CustomerEmail = customerEmail;
        }
    }
}
