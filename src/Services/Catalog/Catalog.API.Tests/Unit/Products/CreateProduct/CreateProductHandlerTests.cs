namespace Catalog.API.Tests.Unit.Products.CreateProduct;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly CreateProductCommandHandler _handler;
    private readonly Faker<CreateProductCommand> _commandFaker;
    public CreateProductHandlerTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _handler = new CreateProductCommandHandler(_mockRepository.Object);
        // Setup Bogus to generate fake CreateProductCommand
        _commandFaker = new Faker<CreateProductCommand>()
            .CustomInstantiator(f => new CreateProductCommand(
                f.Commerce.ProductName(),
                new List<string> { f.Commerce.Categories(1)[0] },
                f.Lorem.Sentence(),
                f.Image.PicsumUrl(),
                f.Random.Decimal(10, 1000)
            ));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_Should_Store_Product_And_Return_Result()
    {
        // Arrange
        // var command = new CreateProductCommand(
        //     Name: "Test Product",
        //     Category: new List<string> { "Books" },
        //     Description: "A sample product",
        //     ImageFile: "image.jpg",
        //     Price: 99.99M
        // );
        var command = _commandFaker.Generate();
         var fakeProductId = Guid.NewGuid();

       _mockRepository
                .Setup(r => r.SaveProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeProductId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();

            _mockRepository.Verify(r => r.SaveProductAsync(It.Is<Product>(p =>
                p.Name == command.Name &&
                p.Category == command.Category &&
                p.Description == command.Description &&
                p.ImageFile == command.ImageFile &&
                p.Price == command.Price
            ), It.IsAny<CancellationToken>()), Times.Once);

            _mockRepository.Verify(r => r.SaveProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);

        }
}
