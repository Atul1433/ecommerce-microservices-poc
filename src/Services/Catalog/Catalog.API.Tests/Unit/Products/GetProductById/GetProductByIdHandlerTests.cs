using Catalog.API.Exceptions;
using Catalog.API.Products.GetProductById;

namespace Catalog.API.Tests.Unit.Products.GetProductById
{
    public class GetProductByIdHandlerTests
    {
        private readonly Faker<Product> _productFaker;
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly GetProductByIdQueryHandler _handler; 
        public GetProductByIdHandlerTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _handler = new GetProductByIdQueryHandler(_mockRepo.Object);
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
        public async Task Handle_Should_Return_Products_Matching_Id()
        {
            // Arrange
            var sampleProduct = _productFaker.Generate();
            var query = new GetProductByIdQuery(sampleProduct.Id);

            _mockRepo
                .Setup(repo => repo.GetProductByIdAsync(sampleProduct.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(sampleProduct);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Product.Should().NotBeNull();
            result.Product.Id.Should().Be(sampleProduct.Id);
            result.Product.Name.Should().Be(sampleProduct.Name);
            result.Product.Category.Should().BeEquivalentTo(sampleProduct.Category);
            result.Product.Price.Should().Be(sampleProduct.Price);

            // Verify repository was called once
            _mockRepo.Verify(r => r.GetProductByIdAsync(sampleProduct.Id, It.IsAny<CancellationToken>()), Times.Once);
        }
        [Fact]
        [Trait("Category", "Unit")]
        public async Task Handle_Should_Throw_ProductNotFoundException_When_Product_Is_Null()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var query = new GetProductByIdQuery(Id: productId);

            _mockRepo.Setup(repo => repo.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((Product?)null);
            

            // Act
            var act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ProductNotFoundException>()
                .WithMessage($"*{productId}*");

           _mockRepo.Verify(r => r.GetProductByIdAsync(productId, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}