
using BuildingBlocks.Exceptions.Handler;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

//Add services to Container
var assembly = typeof(Program).Assembly;
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

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

var app = builder.Build();




//Configure the HTTP Request pipeline.

//Configure the carter
app.MapCarter();
app.UseExceptionHandler(options => { });
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
