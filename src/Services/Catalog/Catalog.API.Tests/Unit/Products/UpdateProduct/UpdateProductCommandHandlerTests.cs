using Catalog.API.Exceptions;
using Catalog.API.Products.UpdateProduct;

namespace Catalog.API.Tests.Unit.Products.UpdateProduct
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly Mock<IDocumentSession> _mockSession;
        private readonly UpdateProductCommandHandler _handler;
        private readonly Faker<Product> _productFaker;

        public UpdateProductCommandHandlerTests()
        {
            _mockSession = new Mock<IDocumentSession>();
            _handler = new UpdateProductCommandHandler(_mockSession.Object);

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
        public async Task Handle_Should_Update_Product_When_Found()
        {
            // Arrange
            var existingProduct = _productFaker.Generate();
            var command = new UpdateProductCommand(
                Id: existingProduct.Id,
                Name: "Updated Product",
                Category: new List<string> { "UpdatedCategory" },
                Description: "Updated Description",
                ImageFile: "updated-image.jpg",
                Price: 999.99m
            );

            _mockSession.Setup(s => s.LoadAsync<Product>(command.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(existingProduct);

            _mockSession.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                        .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            existingProduct.Name.Should().Be(command.Name);
            existingProduct.Description.Should().Be(command.Description);
            existingProduct.Price.Should().Be(command.Price);
            existingProduct.Category.Should().BeEquivalentTo(command.Category);

            _mockSession.Verify(s => s.Update(It.Is<Product>(p => p.Id == command.Id)), Times.Once);
            _mockSession.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task Handle_Should_Throw_When_Product_Not_Found()
        {
            // Arrange
            var command = new UpdateProductCommand(
                Id: Guid.NewGuid(),
                Name: "Nonexistent Product",
                Category: new List<string> { "Unknown" },
                Description: "Does not exist",
                ImageFile: "na.jpg",
                Price: 50
            );

            _mockSession.Setup(s => s.LoadAsync<Product>(command.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((Product?)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ProductNotFoundException>()
                .WithMessage($"*{command.Id}*");

            _mockSession.Verify(s => s.Update(It.IsAny<Product>()), Times.Never);
            _mockSession.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
