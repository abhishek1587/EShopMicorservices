
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

//Add Services to the container

var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddValidatorsFromAssembly(assembly);

builder.Services.AddCarter();

builder.Services.AddMarten(opts =>
{
    // opts.Connection("host=localhost;database=postgres;password=postgres;username=postgres");
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
}).UseLightweightSessions();


//To Seed the document DB
if (builder.Environment.IsDevelopment())
    builder.Services.InitializeMartenWith<CatalogInitialData>();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

//To Health Check
builder.Services.AddHealthChecks().AddNpgSql(builder.Configuration.GetConnectionString("Database")!);


var app = builder.Build();


//Configure the HTTP request pipeline.
app.MapCarter();
app.UseExceptionHandler( opts => {  });
/*
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var excepiton = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (excepiton == null)
        {
            return;
        }
        var problemDetails = new ProblemDetails
        {
            Title = excepiton.Message,
            Status = StatusCodes.Status500InternalServerError,
            Detail = excepiton.StackTrace
        };

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(excepiton, excepiton.Message);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(problemDetails);

    });
});
*/

app.UseHealthChecks("/health", new HealthCheckOptions { 
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.Run();
