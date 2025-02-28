using BuildingBlocks.Exceptions.Handler;
using Discount.gRPC;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);


#region AddServicesToContainer_BeforeBuildOperation
var assembly = typeof(Program).Assembly;

#region ApplicationServices
//Add Carter
builder.Services.AddCarter();
//add Mapster

//Add MediatR for handling the CQRS pattern
builder.Services.AddMediatR(config =>
{

    //provide configuration for MediatR
    config.RegisterServicesFromAssembly(assembly);

    //injecting validation behaviour into request pipeling written in BuildingBlocks using fluentValidation proj
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));

    //add logging behaviour in request pipelin
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
#endregion


#region DataServices
//add Marten service for interactin with postgres document DB
builder.Services.AddMarten(
    opts =>
    {
        opts.Connection(builder.Configuration.GetConnectionString("Database")!);
        opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
    }).UseLightweightSessions();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();

//Manually configuring the CachedBasketRepository
// this way of configuring the CachedBasketRepository is not recommended, instead use the decorator pattern
/*
builder.Services.AddScoped<IBasketRepository>(provider =>
{
    var basketRepository = provider.GetRequiredService<IBasketRepository>();
    return new CachedBasketRepository(basketRepository, provider.GetRequiredService<IDistributedCache>());
});
*/
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis")!;
    //options.InstanceName = "Basket";
});
#endregion

#region GrpcServices
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
})

//ByPass the SSL Certificate 
.ConfigurePrimaryHttpMessageHandler( () =>
{
    var handler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
    return handler;
});

#endregion   


#region Cross-Cutting service
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

#endregion


#endregion


var app = builder.Build();


#region ConfigureTheHTTPRequestPipeline.

//Configure the carter
app.MapCarter();
app.UseExceptionHandler(options => { });
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();

#endregion