using Catalog.API.Models.Dtos;
using Catalog.API.Specifications;

public interface IProductRepository
    {
        Task<Guid> SaveProductAsync(Product product, CancellationToken cancellationToken = default);
        Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken = default);
        Task<bool> DeleteProductAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<List<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken = default);
        Task<List<Product>> GetProductsByBrandAsync(string brand, CancellationToken cancellationToken = default);
        Task<List<Product>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default);
        Task<List<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default);
        Task<List<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<Product?> GetProductByIdAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ProductDto>> GetPagedProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<ProductDto>> GetProductsWithSpecAsync(ISpecification<Product> spec, CancellationToken cancellationToken = default);
        Task<int> CountProductsWithSpecAsync(ISpecification<Product> spec, CancellationToken cancellationToken = default);
    }