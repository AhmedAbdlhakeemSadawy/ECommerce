
using ECommerceDataAccess.DatabaseContextConfiguration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;





var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("ECommerceConnection");
builder.Services.AddECommerceDataAccess(connectionString);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//builder.Services.AddDbContext<ECommerceDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("ECommerceConnection")));




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
