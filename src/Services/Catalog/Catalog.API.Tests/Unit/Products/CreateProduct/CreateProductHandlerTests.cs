namespace Catalog.API.Tests.Unit.Products.CreateProduct;

public class CreateProductHandlerTests
{
    private readonly Mock<IDocumentSession> _mockSession;
    private readonly CreateProductCommandHandler _handler;
    private readonly Faker<CreateProductCommand> _commandFaker;
    public CreateProductHandlerTests()
    {
        _mockSession = new Mock<IDocumentSession>();
        _handler = new CreateProductCommandHandler(_mockSession.Object);
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
        _mockSession.Setup(s => s.Store(It.IsAny<Product[]>()))
        .Callback<Product[]>(products =>
        {
            foreach (var p in products)
            {
                p.Id = Guid.NewGuid(); // simulate ID assignment
            }
        });
        _mockSession.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();

        _mockSession.Verify(s => s.Store(It.Is<Product>(p =>
            p.Name == command.Name &&
            p.Category == command.Category &&
            p.Description == command.Description &&
            p.ImageFile == command.ImageFile &&
            p.Price == command.Price
        )), Times.Once);

        _mockSession.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
