using Catalog.API.Products.GetProducts;

namespace Catalog.API.Tests.Unit.Products.GetProducts
{
    public class GetProductsHandlerTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly GetProductsQueryHandler _handler;
        private readonly Faker<Product> _productFaker;

        public GetProductsHandlerTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _handler = new GetProductsQueryHandler(_mockRepo.Object);

            _productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.ImageFile, f => f.Image.PicsumUrl())
                .RuleFor(p => p.Price, f => f.Random.Decimal(100, 1000))
                .RuleFor(p => p.Category, f => f.Make(1, () => f.PickRandom(new[] { "Electronics", "Furniture", "Clothing" })).ToList());
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Handle_Should_Return_Paged_Products()
        {
            // Arrange
            var sampleProducts = _productFaker.Generate(5);
            var query = new GetProductsQuery(PageNumber: 1, PageSize: 5);

            _mockRepo.Setup(r => r.GetPagedProductsAsync(query.PageNumber!.Value, query.PageSize!.Value, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(sampleProducts);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().HaveCount(5);
            result.Products.Should().BeEquivalentTo(sampleProducts);

            _mockRepo.Verify(r => r.GetPagedProductsAsync(query.PageNumber!.Value, query.PageSize!.Value, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Handle_Should_Return_Empty_When_No_Products_Exist()
        {
            // Arrange
            var query = new GetProductsQuery(PageNumber: 1, PageSize: 5);
            _mockRepo.Setup(r => r.GetPagedProductsAsync(query.PageNumber!.Value, query.PageSize!.Value, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(Enumerable.Empty<Product>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().BeEmpty();

            _mockRepo.Verify(r => r.GetPagedProductsAsync(query.PageNumber!.Value, query.PageSize!.Value, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
