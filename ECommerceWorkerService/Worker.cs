using Azure.Storage.Queues;
using ECommerceBusinessAbstractions;
using ECommerceEvents;
using ECommerceInfrastructure;
using Hangfire;
using Microsoft.Extensions.Hosting;
using System;
using System.Text;
using System.Text.Json;

namespace ECommerceWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly QueueClient queueClient;
        private readonly IServiceProvider serviceProvider;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.queueClient = new QueueClient(
            configuration["AzureStorage:ConnectionString"],
            configuration["AzureStorage:QueueName"]);
            queueClient.CreateIfNotExists();
            this.serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var response = await queueClient.ReceiveMessageAsync(
                    visibilityTimeout: TimeSpan.FromMinutes(2),
                    cancellationToken: stoppingToken);

                var message = response.Value;

                if (message == null)
                {
                    await Task.Delay(2000, stoppingToken);
                    continue;
                }
                try
                {
                    var json = Encoding.UTF8.GetString(Convert.FromBase64String(message.MessageText));

                    var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(json)!;

                    var eventType = orderCreatedEvent.GetType();

                    using var scope = serviceProvider.CreateScope();

                    var handlerType = typeof(IDomainEventHandler<>)
                        .MakeGenericType(eventType);

                    var handlers = scope.ServiceProvider.GetServices(handlerType);

                    foreach (var handler in handlers)
                    {
                        BackgroundJob.Enqueue<OrderCreatedEmailSendEventHandler> (
                            handler => handler.Handle(orderCreatedEvent));
                    }
            }
                catch (Exception exception)
                {
                logger.LogError(exception, "Unexpected error occurred.");
            }



            }
        }
    }
}
