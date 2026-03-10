using Catalog.API.Models.Dtos;
using Catalog.API.Specifications;

namespace Catalog.API.Products.GetProducts;

public record GetProductsQuery(
    int? PageNumber = 1,
    int? PageSize = 10,
    string? Sort = null,
    string? Search = null,
    string? Brand = null,
    string? Category = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? IsFeatured = null) : IQuery<GetProductsResult>;

public record GetProductsResult(
    IEnumerable<ProductDto> Products,
    int PageIndex,
    int PageSize,
    int Count,
    int TotalCount);

public class GetProductsQueryHandler
    (IProductRepository repository)
    : IQueryHandler<GetProductsQuery, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var specParams = new ProductSpecParams
        {
            PageIndex = query.PageNumber ?? 1,
            PageSize = query.PageSize ?? 10,
            Sort = query.Sort,
            Search = query.Search,
            Brand = query.Brand,
            Category = query.Category,
            MinPrice = query.MinPrice,
            MaxPrice = query.MaxPrice,
            IsFeatured = query.IsFeatured
        };

        var spec = new ProductSpecification(specParams);

        var products = await repository.GetProductsWithSpecAsync(spec, cancellationToken);
        var count = await repository.CountProductsWithSpecAsync(spec, cancellationToken);

        return new GetProductsResult(
            products,
            specParams.PageIndex,
            specParams.PageSize,
            products.Count(),
            count);
    }
}
