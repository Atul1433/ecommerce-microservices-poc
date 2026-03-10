namespace Catalog.API.Models.Dtos;

/// <summary>
/// DTO for product list views (home page, search results)
/// Contains essential product information for display without unnecessary detail
/// </summary>
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal DiscountedPrice { get; set; }
    public string ImageFile { get; set; } = default!;
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public bool IsFeatured { get; set; }
    public string CategoryName { get; set; } = default!;
    public string BrandName { get; set; } = default!;
}
