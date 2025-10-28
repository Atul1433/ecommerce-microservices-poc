

namespace Catalog.API.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDocumentSession _session;
        public ProductRepository(IDocumentSession session)
        {
            _session = session;
        }

        public async Task<bool> DeleteProductAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            _session.Delete<Product>(Id);
            await _session.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IEnumerable<Product>> GetPagedProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            return await _session.Query<Product>()
            .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
        }

        public async Task<Product?> GetProductByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await _session.LoadAsync<Product>(Id, cancellationToken);
        }

        public async Task<List<Product>> GetProductsByCatagoryAsync(string catagory, CancellationToken cancellationToken = default)
        {
            var product = await _session.Query<Product>()
            .Where(p => p.Category.Contains(catagory))
            .ToListAsync(cancellationToken);
            return product.ToList();
        }

        public async Task<Guid> SaveProductAsync(Product product, CancellationToken cancellationToken = default)
        {
            _session.Store(product);
            await _session.SaveChangesAsync(cancellationToken);
            return product.Id;
        }

        public async Task<bool> UpdateProductAsync(Product product, CancellationToken cancellationToken = default)
        {
            _session.Update(product);
            await _session.SaveChangesAsync(cancellationToken);
            return true;

        }
    }
}