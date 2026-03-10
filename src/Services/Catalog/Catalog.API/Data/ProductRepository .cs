using System;
using Catalog.API.Models.Dtos;
using Catalog.API.Specifications;

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

        public async Task<IEnumerable<ProductDto>> GetPagedProductsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var categories = new List<Category>();
            var brands = new List<Brand>();
            var products = await _session.Query<Product>()
         .Include<Category>(x => x.CategoryId, categories.Add)
        .Include<Brand>(x => x.BrandId, brands.Add)
        .ToPagedListAsync(pageNumber, pageSize, cancellationToken);

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DiscountPercentage = p.DiscountPercentage,
                DiscountedPrice = p.DiscountedPrice,
                ImageFile = p.ImageFile,
                AverageRating = p.AverageRating,
                TotalReviews = p.TotalReviews,
                IsFeatured = p.IsFeatured,
                CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name ?? string.Empty,
                BrandName = brands.FirstOrDefault(b => b.Id == p.BrandId)?.Name ?? string.Empty
            });

            return productDtos;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsWithSpecAsync(ISpecification<Product> spec, CancellationToken cancellationToken = default)
        {
            var categories = new List<Category>();
            var brands = new List<Brand>();

            var query = _session.Query<Product>()
                .Include<Category>(x => x.CategoryId, categories.Add)
                .Include<Brand>(x => x.BrandId, brands.Add);

            // Apply criteria
            if (spec.Criteria != null)
            {
                query = (Marten.Linq.IMartenQueryable<Product>)query.Where(spec.Criteria);
            }

            // Apply ordering
            if (spec.OrderBy != null)
            {
                query = (Marten.Linq.IMartenQueryable<Product>)query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                query = (Marten.Linq.IMartenQueryable<Product>)query.OrderByDescending(spec.OrderByDescending);
            }

            // Apply paging
            if (spec.IsPagingEnabled)
            {
                query = (Marten.Linq.IMartenQueryable<Product>)query.Skip(spec.Skip).Take(spec.Take);
            }

            var products = await query.ToListAsync(cancellationToken);

            var productDtos = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DiscountPercentage = p.DiscountPercentage,
                DiscountedPrice = p.DiscountedPrice,
                ImageFile = p.ImageFile,
                AverageRating = p.AverageRating,
                TotalReviews = p.TotalReviews,
                IsFeatured = p.IsFeatured,
                CategoryName = categories.FirstOrDefault(c => c.Id == p.CategoryId)?.Name ?? string.Empty,
                BrandName = brands.FirstOrDefault(b => b.Id == p.BrandId)?.Name ?? string.Empty
            });

            return productDtos;
        }

        public async Task<int> CountProductsWithSpecAsync(ISpecification<Product> spec, CancellationToken cancellationToken = default)
        {
            var query = _session.Query<Product>();

            if (spec.Criteria != null)
            {
                query = (Marten.Linq.IMartenQueryable<Product>)query.Where(spec.Criteria);
            }

            return await query.CountAsync(cancellationToken);
        }

        public async Task<Product?> GetProductByIdAsync(Guid Id, CancellationToken cancellationToken = default)
        {
            return await _session.LoadAsync<Product>(Id, cancellationToken);
        }

        public async Task<List<Product>> GetProductsByBrandAsync(string brand, CancellationToken cancellationToken = default)
        {
            return (await _session.Query<Product>()
                .Where(p => p.Brand != null && p.Brand.Name.Contains(brand, StringComparison.OrdinalIgnoreCase))
                .ToListAsync(cancellationToken)).ToList();
        }

        public async Task<List<Product>> GetFeaturedProductsAsync(CancellationToken cancellationToken = default)
        {
            return (await _session.Query<Product>()
                .Where(p => p.IsFeatured && p.IsActive)
                .ToListAsync(cancellationToken)).ToList();
        }

        public async Task<List<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice, CancellationToken cancellationToken = default)
        {
            return (await _session.Query<Product>()
                .Where(p => p.DiscountedPrice >= minPrice && p.DiscountedPrice <= maxPrice && p.IsActive)
                .ToListAsync(cancellationToken)).ToList();
        }

        public async Task<List<Product>> SearchProductsAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var term = searchTerm.ToLowerInvariant();
            return (await _session.Query<Product>()
                .Where(p => p.IsActive && (
                    p.Name.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term) ||
                    p.LongDescription.ToLower().Contains(term) ||
                    p.SKU.ToLower().Contains(term) ||
                    p.Tags.Any(t => t.ToLower().Contains(term)) ||
                    p.Keywords.Any(k => k.ToLower().Contains(term)) ||
                    (p.Category != null && p.Category.Name.ToLower().Contains(term)) ||
                    (p.Brand != null && p.Brand.Name.ToLower().Contains(term))
                ))
                .ToListAsync(cancellationToken)).ToList();
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

        public Task<List<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}