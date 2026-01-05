using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using ECommerceBusinessAbstractions;
using ECommerceEvents;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Text.Json;

namespace ECommerceOrderAzureFunction
{
    public class HandleOrderMessages
    {
        private readonly ILogger<HandleOrderMessages> logger;
        private readonly IServiceProvider serviceProvider;

        public HandleOrderMessages(ILogger<HandleOrderMessages> logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        [Function(nameof(HandleOrderMessages))]
        public async Task Run([QueueTrigger("orders", Connection = "StorageConnection")] QueueMessage message)
        {


            try
            {
               // var json = Encoding.UTF8.GetString(Convert.FromBase64String(message.MessageText));

                var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(message.MessageText)!;

                var eventType = orderCreatedEvent.GetType();

                using var scope = serviceProvider.CreateScope();

                var handlerType = typeof(IDomainEventHandler<>)
                    .MakeGenericType(eventType);

                var handlers = scope.ServiceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    await ((dynamic)handler)
                        .Handle((dynamic)orderCreatedEvent);
                }

            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected error occurred.");
            }

            
        }
    }
}
