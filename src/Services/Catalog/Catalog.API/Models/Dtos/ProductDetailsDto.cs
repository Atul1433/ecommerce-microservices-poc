namespace Catalog.API.Models.Dtos;

/// <summary>
/// DTO for detailed product view (product detail page)
/// Contains complete product information including specifications, inventory, and media
/// </summary>
public class ProductDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string SKU { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string LongDescription { get; set; } = default!;
    
    // Pricing
    public decimal Price { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal DiscountedPrice { get; set; }
    
    // Images and Media
    public string ImageFile { get; set; } = default!;
    public List<string> AdditionalImages { get; set; } = new();
    
    // Inventory
    public int StockQuantity { get; set; }
    public string SKUStatus { get; set; } = default!;
    
    // Physical Specifications
    public decimal Weight { get; set; }
    public string Dimensions { get; set; } = default!;
    public string Color { get; set; } = default!;
    public string Material { get; set; } = default!;
    
    // Ratings and Reviews
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    
    // Tags and Search
    public List<string> Tags { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    
    // Status
    public bool IsActive { get; set; }
    
    // Category and Brand
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public Guid BrandId { get; set; }
    public string BrandName { get; set; } = default!;
    
    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
