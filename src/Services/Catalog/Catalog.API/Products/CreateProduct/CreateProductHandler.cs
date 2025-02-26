
namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price)
        :ICommand<CreateProductResult>;
    public  record CreateProductResult(Guid Id);


    public class CreatePrductCommandValidator: AbstractValidator<CreateProductCommand>
    {
        public CreatePrductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is required");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero");
        }
    }
    internal class CreateProductCommandHandler (
        IDocumentSession session
        //, IValidator<CreateProductCommand> validator
        //,ILogger<CreateProductCommandHandler> logger
        )
        : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {


          //  logger.LogInformation("CreateProductCommandHandler.Handle called with {@command} ",command);
            /*
            //Validate the command object
            var result = await validator.ValidateAsync(command, cancellationToken);
            var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
            if (errors.Count != 0) { 
                throw new ValidationException(errors.FirstOrDefault());
            }
            */
            //Business Logic to create a product

            //Create product entity from command object
            //Save to database
            //return CreateProductResult with Id

            var product = new Product
            {
                Name = command.Name,
                Category = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price
            };


            //Save to database
            session.Store(product);
            await session.SaveChangesAsync(cancellationToken);
            //return CreateProductResult with Id

            return  new CreateProductResult(product.Id);
            //throw new NotImplementedException();
        }
    }
}
