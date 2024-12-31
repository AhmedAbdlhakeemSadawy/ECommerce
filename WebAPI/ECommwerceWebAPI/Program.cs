
using AutoMapper;
using ECommerceBusinessLogic.ECommerceBusinessServiceRegisteration;
using ECommerceBusinessLogic.Mapping_Profiles;
using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataSeeder;
using ECommerceDataAccessAbstraction;
using ECommwerceWebAPI.Mapping_Profiles;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;





var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("ECommerceConnection");
builder.Services.AddECommerceDataAccess(connectionString);
builder.Services.RegisterBusinessServices();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new ProductMappingProfile()); // Add your profiles here
    cfg.AddProfile(new OrderAPIMappingProfile()); // Add your profiles here
}).CreateMapper());
builder.Services.AddScoped<IDataSeeder, DataSeeder>();
var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    dataSeeder.SeedData();
}

app.Run();
