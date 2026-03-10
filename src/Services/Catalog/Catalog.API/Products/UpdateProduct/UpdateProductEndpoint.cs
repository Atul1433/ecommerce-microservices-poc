
namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductRequest(
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
    bool IsFeatured = false);

public record UpdateProductResponse(bool IsSuccess);

public class UpdateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/products", 
            async (UpdateProductRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateProductCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<UpdateProductResponse>();

                return Results.Ok(response);
            })
            .WithName("UpdateProduct")
            .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Update Product")
            .WithDescription("Update Product");
    }
}
