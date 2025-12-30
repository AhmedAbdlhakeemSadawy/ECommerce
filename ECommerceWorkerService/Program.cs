using ECommerceBusinessAbstractions;
using ECommerceEvents;
using ECommerceInfrastructure;
using ECommerceInfrastructureAbstraction;
using ECommerceWorkerService;

//var builder = Host.CreateApplicationBuilder(args);


//var host = builder.Build();
//host.Run();


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Scoped handlers
        services.AddScoped<IEmailService, AzureCommunicationEmailService>();
        services.AddScoped<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedEmailSendEventHandler>();


        // Background service
        services.AddHostedService<Worker>();
        //services.Configure<HostOptions>(options =>
        //{
        //    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
        //});
    })
    .Build();

await host.RunAsync();