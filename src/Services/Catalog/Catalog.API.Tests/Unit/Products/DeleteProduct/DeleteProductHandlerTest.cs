
using Catalog.API.Products.DeleteProduct;

namespace Catalog.API.Tests.Unit.Products.DeleteProduct;
public class DeleteProductHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductHandlerTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _handler = new DeleteProductCommandHandler(_mockRepository.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_Should_Delete_Product_And_Return_Success()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockRepository.Setup(r => r.DeleteProductAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        

        var command = new DeleteProductCommand(productId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteProductAsync(productId, It.IsAny<CancellationToken>()), Times.Once);

   }
}