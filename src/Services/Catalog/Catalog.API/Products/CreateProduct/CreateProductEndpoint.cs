namespace Catalog.API.Products.CreateProduct;

public record CreateProductRequest(
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
    bool IsFeatured = false);

public record CreateProductResponse(Guid Id);

public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/products",
            async (CreateProductRequest request, ISender sender) =>
        {
            var command = request.Adapt<CreateProductCommand>();

            var result = await sender.Send(command);

            var response = result.Adapt<CreateProductResponse>();

            return Results.Created($"/products/{response.Id}", response);

        })
        .WithName("CreateProduct")
        .Produces<CreateProductResponse>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Create Product")
        .WithDescription("Create Product");
    }
}
