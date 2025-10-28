namespace Catalog.API.Data
{
    public interface IProductRepository
    {
        Task<Guid> SaveProductAsync(Product product, CancellationToken cancellationToken = default);
        Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken = default);
        Task<bool> DeleteProductAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<List<Product>> GetProductsByCatagoryAsync(string catagory, CancellationToken cancellationToken = default);
        Task<Product?> GetProductByIdAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetPagedProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    }
}