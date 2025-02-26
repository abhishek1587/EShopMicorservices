
namespace Catalog.API.Products.GetProducts
{


    //Request
    public record GetProductRequest(int? PageNumber=1, int? PageSize=10);

    //Response
    public record GetProductResponse(IEnumerable<Product> Products);
    public class GetProductsEndpoint() : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async ([AsParameters] GetProductRequest request, ISender sender) =>
            {
                var query = request.Adapt<GetProductsQuery>();
                //var result = await sender.Send(new GetProductsQuery());
                var result = await sender.Send(query);
                var response = result.Adapt<GetProductResponse>();
                return Results.Ok(response);
            })
                 .WithName("GetProducts")
                 .Produces<GetProductResponse>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .WithSummary("Get all products")
                 .WithDescription("Get all products in the catalog");
        }
    }
}
