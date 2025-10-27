
using Catalog.API.Products.DeleteProduct;

namespace Catalog.API.Tests.Unit.Products.DeleteProduct;
public class DeleteProductHandlerTests
{
    private readonly Mock<IDocumentSession> _mockSession;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductHandlerTests()
    {
        _mockSession = new Mock<IDocumentSession>();
        _handler = new DeleteProductCommandHandler(_mockSession.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_Should_Delete_Product_And_Return_Success()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockSession.Setup(s => s.Delete<Product>(productId));
        _mockSession.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        var command = new DeleteProductCommand(productId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();

        _mockSession.Verify(s => s.Delete<Product>(productId), Times.Once);
        _mockSession.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}