namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    string SKU,
    Guid CategoryId,
    Guid BrandId,
    string Description,
    string LongDescription,
    string ImageFile,
    List<string> AdditionalImages,
    decimal Price,
    decimal? CostPrice,
    decimal? DiscountPercentage,
    int StockQuantity,
    int ReorderLevel,
    decimal Weight,
    string Dimensions,
    string Color,
    string Material,
    List<string> Tags,
    List<string> Keywords,
    bool IsActive = true,
    bool IsFeatured = false)
    : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(2, 150).WithMessage("Name must be between 2 and 150 characters");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required")
            .Length(2, 50).WithMessage("SKU must be between 2 and 50 characters");

        RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Category is required");
        RuleFor(x => x.BrandId).NotEmpty().WithMessage("Brand is required");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is required");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.DiscountPercentage)
            .GreaterThanOrEqualTo(0).WithMessage("Discount percentage must be 0 or greater")
            .LessThanOrEqualTo(100).WithMessage("Discount percentage cannot exceed 100");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

        RuleFor(x => x.Weight)
            .GreaterThanOrEqualTo(0).WithMessage("Weight cannot be negative");
    }
}

public class CreateProductCommandHandler
    (IProductRepository repository)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = command.Name,
            SKU = command.SKU,
            CategoryId = command.CategoryId,
            BrandId = command.BrandId,
            Description = command.Description,
            LongDescription = command.LongDescription,
            ImageFile = command.ImageFile,
            AdditionalImages = command.AdditionalImages ?? new(),
            Price = command.Price,
            CostPrice = command.CostPrice,
            DiscountPercentage = command.DiscountPercentage,
            StockQuantity = command.StockQuantity,
            ReorderLevel = command.ReorderLevel,
            Weight = command.Weight,
            Dimensions = command.Dimensions,
            Color = command.Color,
            Material = command.Material,
            Tags = command.Tags ?? new(),
            Keywords = command.Keywords ?? new(),
            IsActive = command.IsActive,
            IsFeatured = command.IsFeatured,
            CreatedAt = DateTime.UtcNow
        };

        var productId = await repository.SaveProductAsync(product, cancellationToken);
        return new CreateProductResult(productId);
    }
}
