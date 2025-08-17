using ECommerceBusinessAbstractions;
using ECommerceEvents;
using ECommerceInfrastructureAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceInfrastructure
{
    public class OrderCreatedEmailSendEventHandler  : IDomainEventHandler<OrderCreatedEvent>
    {
        private readonly IEmailService emailService;
        public OrderCreatedEmailSendEventHandler(IEmailService emailService)
        {
            this.emailService = emailService;
        }
        public async Task Handle(OrderCreatedEvent orderCreatedEvent)
        {
            string content = $"<h1>Order Confirmation</h1><p> Kindly note that Your order number {orderCreatedEvent.OrderNumber}  has been received.</p>";
            await emailService.SendEmailAsync(orderCreatedEvent.CustomerEmail, "Order Confirmation", content);

        }
    }
}
