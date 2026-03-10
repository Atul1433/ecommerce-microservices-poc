namespace Catalog.API.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string SKU { get; set; } = default!;  // Stock Keeping Unit
    public string Description { get; set; } = default!;
    public string LongDescription { get; set; } = default!;
    
    // Pricing
    public decimal Price { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal DiscountedPrice => DiscountPercentage.HasValue 
        ? Price * (1 - (DiscountPercentage.Value / 100)) 
        : Price;
    
    // Category and Brand
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public Guid BrandId { get; set; }
    public Brand? Brand { get; set; }
    
    // Images and Media
    public string ImageFile { get; set; } = default!;
    public List<string> AdditionalImages { get; set; } = new();
    
    // Inventory
    public int StockQuantity { get; set; }
    public int ReorderLevel { get; set; } = 10;  // Minimum quantity before reorder
    public string SKUStatus { get; set; } = "Active";  // Active, Discontinued, Inactive
    
    // Physical Specifications
    public decimal Weight { get; set; }  // in kg
    public string Dimensions { get; set; } = default!;  // Length x Width x Height
    public string Color { get; set; } = default!;
    public string Material { get; set; } = default!;
    
    // Ratings and Reviews
    public decimal AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    
    // Tags and Search
    public List<string> Tags { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    
    // Audit and Status
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
