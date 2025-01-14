
using AutoMapper;
using ECommerceBusinessLogic.ECommerceBusinessServiceRegisteration;
using ECommerceBusinessLogic.Mapping_Profiles;
using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataSeeder;
using ECommerceDataAccess.Mapping_Profiles;
using ECommerceDataAccessAbstraction;
using ECommerceWebApiDto.Validators;
using ECommwerceWebAPI.Mapping_Profiles;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;





var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("ECommerceConnection");
builder.Services.AddECommerceDataAccess(connectionString);
builder.Services.RegisterBusinessServices();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.SignIn.RequireConfirmedEmail = false; // Change as needed
})
.AddEntityFrameworkStores<ECommerceDbContext>()
.AddDefaultTokenProviders();
// Add services to the container.

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Enforce HTTPS
    options.Cookie.SameSite = SameSiteMode.Lax; // Protect against CSRF
    options.LoginPath = "/api/Account/login"; // Redirect path for login
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    //options.LogoutPath = "/Account/Logout"; // Redirect path for logout
    options.AccessDeniedPath = "/api/Account/accessdenied"; // Redirect for unauthorized access
});

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<OrderRequestDtoValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(provider => new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new ProductMappingProfile()); // Add your profiles here
    cfg.AddProfile(new OrderAPIMappingProfile()); // Add your profiles here
    cfg.AddProfile(new OrderDataMappingProfile()); // Add your profiles here
    cfg.AddProfile(new OrderMappingProfile()); // Add your profiles here
    cfg.AddProfile(new ProductDataMappingProfile()); // Add your profiles here
}).CreateMapper());
builder.Services.AddScoped<IDataSeeder, DataSeeder>();
var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    dataSeeder.SeedData();
}

app.Run();
