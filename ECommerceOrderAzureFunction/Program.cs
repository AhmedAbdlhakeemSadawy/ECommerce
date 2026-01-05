using ECommerceBusinessAbstractions;
using ECommerceEvents;
using ECommerceInfrastructure;
using ECommerceInfrastructureAbstraction;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context,services) =>
    {
        services.AddScoped<IEmailService, AzureCommunicationEmailService>();
        services.AddScoped<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedEmailSendEventHandler>();
        services.Configure<AzureEmailCommunicationSettings>(context.Configuration.GetSection("AzureEmailCommunicationSettings"));

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
