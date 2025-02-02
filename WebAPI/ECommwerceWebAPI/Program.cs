
using AutoMapper;
using ECommerceBusinessLogic.ECommerceBusinessServiceRegisteration;
using ECommerceBusinessLogic.Mapping_Profiles;
using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataSeeder;
using ECommerceDataAccess.Mapping_Profiles;
using ECommerceDataAccessAbstraction;
using ECommerceWebApiDto.Validators;
using ECommwerceWebAPI.Mapping_Profiles;
using ECommwerceWebAPI.Middlewares;
using ECommwerceWebAPI.Role_Requirements_Authorization;
using ECommwerceWebAPI.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebApiAbstraction;
using WebApiAbstraction.Role_Authuntication;





var builder = WebApplication.CreateBuilder(args);



var connectionString = builder.Configuration.GetConnectionString("ECommerceConnection");
builder.Services.AddECommerceDataAccess(connectionString);
builder.Services.RegisterBusinessServices();
//builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITokenService, TokenServiceInMemoryCache>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
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

//#region Cookie based athentication configration
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.HttpOnly = true;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Enforce HTTPS
//    options.Cookie.SameSite = SameSiteMode.Lax; // Protect against CSRF
//    options.LoginPath = "/api/Account/login"; // Redirect path for login
//    options.Events.OnRedirectToLogin = context =>
//    {
//        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

//        // Set the response content type to JSON
//        context.Response.ContentType = "application/json";

//        // Write a custom JSON message to the response body
//        var responseMessage = new
//        {
//            message = "You are not authorized to access this resource. Please log in."
//        };

//        return context.Response.WriteAsJsonAsync(responseMessage);
//    };
//    //options.LogoutPath = "/Account/Logout"; // Redirect path for logout
//    options.AccessDeniedPath = "/api/Account/accessdenied"; // Redirect for unauthorized access
//});
//#endregion

#region configration for jwt based authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],  // E.g., "https://yourapi.com"

            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],  // E.g., "https://yourapi.com"

            ValidateLifetime = true,  // Ensure token is not expired
            ClockSkew = TimeSpan.Zero,  // Adjust for clock skew

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))  // The signing key
        };
    });
#endregion

//builder.Services.AddStackExchangeRedisCache(options =>
//{
//    options.Configuration = "localhost:6379"; // Your Redis connection string
//});

builder.Services.AddMemoryCache();
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


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy =>
        policy.Requirements.Add(new RoleRequirement("User")));

});

builder.Services.AddScoped<IAuthorizationHandler, RoleRequirementHandler>();


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer",
        Description = "Enter 'Bearer' followed by a space and then your JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AccessTokenValidationMiddleware>();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await dataSeeder.SeedData();
}

app.Run();
