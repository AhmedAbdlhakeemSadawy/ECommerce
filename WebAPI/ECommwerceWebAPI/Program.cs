
using AutoMapper;
using ECommerceBusinessAbstractions;
using ECommerceBusinessLogic.ECommerceBusinessServiceRegisteration;
using ECommerceBusinessLogic.Mapping_Profiles;
using ECommerceDataAccess.DatabaseContextConfiguration;
using ECommerceDataAccess.DataSeeder;
using ECommerceDataAccess.Mapping_Profiles;
using ECommerceDataAccessAbstraction;
using ECommerceEvents;
using ECommerceInfrastructure;
using ECommerceInfrastructureAbstraction;
using ECommerceWebApiDto.Validators;
using ECommwerceWebAPI.Filters;
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
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Text;
using WebApiAbstraction;
using WebApiAbstraction.Role_Authuntication;





var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .MinimumLevel.Information() // Set default log level
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) // Reduce noise from Microsoft logs
        .Enrich.FromLogContext()
        .WriteTo.Console()// Add context like request IDs // Log to console for local debugging and Azure Log Stream
        .WriteTo.File(
            path: "logs/app-.txt",
            rollingInterval: RollingInterval.Day, // Daily log files
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"); // Structured file output

    // Optional: Add Application Insights for Azure (uncomment and add connection string in appsettings.json)
    /*
    .WriteTo.ApplicationInsights(
        services.GetRequiredService<Microsoft.ApplicationInsights.TelemetryClient>(),
        Serilog.Sinks.ApplicationInsights.TelemetryConverters.TelemetryConverter.Traces)
    */
});

var connectionString = builder.Configuration.GetConnectionString("ECommerceConnection");
builder.Services.AddECommerceDataAccess(connectionString);
builder.Services.RegisterBusinessServices();
//builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, AzureCommunicationEmailService>();
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



if (builder.Environment.IsProduction())
{
    builder.Services.AddSingleton<ITokenService, TokenServiceRedis>();

    // Redis caching for Production
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString") ?? "localhost:6379";
        // Optional: If you set a password on the sidecar, add it here, e.g., ",password=yourpassword"
    });
}
else
{
    builder.Services.AddScoped<ITokenService, TokenServiceInMemoryCache>();

    // In-memory caching for Development/others
    builder.Services.AddMemoryCache();
}


builder.Services.AddControllers(options =>
    options.Filters.Add<SuccessResponseFilter>());
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<OrderRequestDtoValidator>();
builder.Services.AddScoped<AccessTokenValidationMiddleware>();
builder.Services.Configure<AzureEmailCommunicationSettings>(builder.Configuration.GetSection("AzureEmailCommunicationSettings"));
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();

builder.Services.AddScoped<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedEmailSendEventHandler>();

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
var UiApplicationUrl = builder.Configuration["UIUrl"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", builder =>
    {
        builder.WithOrigins(UiApplicationUrl) // Your Angular app's URL
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });

});

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
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
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
