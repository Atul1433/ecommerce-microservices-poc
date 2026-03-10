using Catalog.API.Models;

namespace Catalog.API.Specifications;

public class ProductSpecification : BaseSpecification<Product>
{
    public ProductSpecification(ProductSpecParams productParams)
    {
        // Always include active products
        Criteria = p => p.IsActive;

        // Apply filters
        if (!string.IsNullOrEmpty(productParams.Search))
        {
            var searchTerm = productParams.Search.ToLowerInvariant();
            AddCriteria(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                p.Description.ToLower().Contains(searchTerm) ||
                p.LongDescription.ToLower().Contains(searchTerm) ||
                p.SKU.ToLower().Contains(searchTerm) ||
                p.Tags.Any(t => t.ToLower().Contains(searchTerm)) ||
                p.Keywords.Any(k => k.ToLower().Contains(searchTerm)) ||
                (p.Category != null && p.Category.Name.ToLower().Contains(searchTerm)) ||
                (p.Brand != null && p.Brand.Name.ToLower().Contains(searchTerm))
            );
        }

        if (!string.IsNullOrEmpty(productParams.Brand))
        {
            AddCriteria(p => p.Brand != null && p.Brand.Name.ToLower().Contains(productParams.Brand.ToLower()));
        }

        if (!string.IsNullOrEmpty(productParams.Category))
        {
            AddCriteria(p => p.Category != null && p.Category.Name.ToLower().Contains(productParams.Category.ToLower()));
        }

        if (productParams.MinPrice.HasValue)
        {
            AddCriteria(p => p.DiscountedPrice >= productParams.MinPrice.Value);
        }

        if (productParams.MaxPrice.HasValue)
        {
            AddCriteria(p => p.DiscountedPrice <= productParams.MaxPrice.Value);
        }

        if (productParams.IsFeatured.HasValue)
        {
            AddCriteria(p => p.IsFeatured == productParams.IsFeatured.Value);
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(productParams.Sort))
        {
            switch (productParams.Sort.ToLower())
            {
                case "priceasc":
                    ApplyOrderBy(p => p.DiscountedPrice);
                    break;
                case "pricedesc":
                    ApplyOrderByDescending(p => p.DiscountedPrice);
                    break;
                case "nameasc":
                    ApplyOrderBy(p => p.Name);
                    break;
                case "namedesc":
                    ApplyOrderByDescending(p => p.Name);
                    break;
                case "ratingdesc":
                    ApplyOrderByDescending(p => p.AverageRating);
                    break;
                case "newest":
                    ApplyOrderByDescending(p => p.CreatedAt);
                    break;
                case "oldest":
                    ApplyOrderBy(p => p.CreatedAt);
                    break;
                default:
                    ApplyOrderBy(p => p.Name);
                    break;
            }
        }
        else
        {
            ApplyOrderBy(p => p.Name);
        }

        // Apply paging
        ApplyPaging(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);
    }
}

public class ProductSpecParams
{
    private const int MaxPageSize = 50;
    public int PageIndex { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
    }

    public string? Sort { get; set; }
    public string? Search { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsFeatured { get; set; }
}