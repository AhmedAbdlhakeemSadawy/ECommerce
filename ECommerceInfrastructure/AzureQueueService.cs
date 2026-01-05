using Azure.Storage.Queues;
using ECommerceBusinessAbstractions;
using ECommerceInfrastructureAbstraction;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerceInfrastructure
{
    public class AzureQueueService : IEventBus
    {
        private readonly QueueClient queueClient;
        public AzureQueueService(IConfiguration configuration)
        {
            var connectionString = configuration["AzureStorage:ConnectionString"];
            var queueName = configuration["AzureStorage:QueueName"];

            queueClient = new QueueClient(connectionString, queueName);
            queueClient.CreateIfNotExists();
        }
        public async Task Publish<T>(T domainEvent) where T : IDomainEvent
        {
            string json = JsonSerializer.Serialize(domainEvent);
            await queueClient.SendMessageAsync(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json)));
        
        }
    }
}
