
namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
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
    : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty().WithMessage("Product ID is required");

        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Name is required")
            .Length(2, 150).WithMessage("Name must be between 2 and 150 characters");

        RuleFor(command => command.SKU)
            .NotEmpty().WithMessage("SKU is required")
            .Length(2, 50).WithMessage("SKU must be between 2 and 50 characters");

        RuleFor(command => command.CategoryId).NotEmpty().WithMessage("Category is required");
        RuleFor(command => command.BrandId).NotEmpty().WithMessage("Brand is required");

        RuleFor(command => command.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(command => command.DiscountPercentage)
            .GreaterThanOrEqualTo(0).WithMessage("Discount percentage must be 0 or greater")
            .LessThanOrEqualTo(100).WithMessage("Discount percentage cannot exceed 100");

        RuleFor(command => command.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

        RuleFor(command => command.Weight)
            .GreaterThanOrEqualTo(0).WithMessage("Weight cannot be negative");
    }
}

public class UpdateProductCommandHandler
    (IDocumentSession session)
    : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await session.LoadAsync<Product>(command.Id, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(command.Id);
        }

        product.Name = command.Name;
        product.SKU = command.SKU;
        product.CategoryId = command.CategoryId;
        product.BrandId = command.BrandId;
        product.Description = command.Description;
        product.LongDescription = command.LongDescription;
        product.ImageFile = command.ImageFile;
        product.AdditionalImages = command.AdditionalImages ?? new();
        product.Price = command.Price;
        product.CostPrice = command.CostPrice;
        product.DiscountPercentage = command.DiscountPercentage;
        product.StockQuantity = command.StockQuantity;
        product.ReorderLevel = command.ReorderLevel;
        product.Weight = command.Weight;
        product.Dimensions = command.Dimensions;
        product.Color = command.Color;
        product.Material = command.Material;
        product.Tags = command.Tags ?? new();
        product.Keywords = command.Keywords ?? new();
        product.IsActive = command.IsActive;
        product.IsFeatured = command.IsFeatured;
        product.UpdatedAt = DateTime.UtcNow;

        session.Update(product);
        await session.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);
    }
}