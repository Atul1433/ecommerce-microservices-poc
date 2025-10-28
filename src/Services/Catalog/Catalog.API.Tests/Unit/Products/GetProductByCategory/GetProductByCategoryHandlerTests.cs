using Catalog.API.Products.GetProductByCategory;


namespace Catalog.API.Tests.Unit.Products.GetProductByCategory
{
    public class GetProductByCategoryHandlerTests
    {
        private readonly Faker<Product> _productFaker;
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly GetProductByCategoryQueryHandler _handler;

        public GetProductByCategoryHandlerTests()
        {
            _mockRepo = new Mock<IProductRepository>();

            _productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.ImageFile, f => f.Image.PicsumUrl())
                .RuleFor(p => p.Price, f => f.Random.Decimal(100, 1000))
                .RuleFor(p => p.Category, f => f.Make(1, () => f.PickRandom(new[] { "Electronics", "Furniture", "Clothing" })).ToList());

            _handler = new GetProductByCategoryQueryHandler(_mockRepo.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Products_Matching_Category()
        {
            // Arrange
            var sampleProducts = _productFaker.Generate(10);
            var expected = sampleProducts.Where(p => p.Category.Contains("Electronics")).ToList();

            _mockRepo.Setup(r => r.GetProductsByCatagoryAsync("Electronics", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(expected);

            var query = new GetProductByCategoryQuery("Electronics");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_When_No_Match()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetProductsByCatagoryAsync("Unknown", It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<Product>());

            var query = new GetProductByCategoryQuery("Unknown");

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Products.Should().BeEmpty();
        }
    }
}
