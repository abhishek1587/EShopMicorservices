
namespace Catalog.API.Products.CreateProduct
{

    public record CreateProductRequest(string Name, List<string> Category, string Description, string ImageFile, decimal Price);

    public record CreateProductResponse(Guid Id);
    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {

            //We are using the Carter library to create the endpoint
            app.MapPost("/products",
                async (CreateProductRequest request, ISender sender) =>
            {
                //using the mapster library to convert the request object to the command object
                var command = request.Adapt<CreateProductCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<CreateProductResponse>();
                return Results.Created($"/products/{response.Id}", response);
            })
            .WithName("CreateProduct")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Creates a new product")
            .WithDescription("Creates a new product in the catalog");

            /*

            //Without using Carter library
            app.Post("/products", async (req, res) =>
            {
                var request = await req.Bind<CreateProductRequest>();
                var command = new CreateProductCommand(request.Name, request.Category, request.Description, request.ImageFile, request.Price);
                var result = await req.Send(command);
                await res.AsJson(new CreateProductResponse(result.Id));
            });
            */
        }
    }
}
