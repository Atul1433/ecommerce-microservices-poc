using Catalog.API.Products.DeleteProduct;

namespace Catalog.API.Tests.Unit.Products.DeleteProduct
{
    public class DeleteProductCommandValidatorTests
    {
        private readonly DeleteProductCommandValidator _validator = new();

        [Fact]
        public void Should_Fail_When_Id_Is_Empty()
        {
            var command = new DeleteProductCommand(Guid.Empty);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Id");
        }

        [Fact]
        public void Should_Pass_When_Id_Is_Valid()
        {
            var command = new DeleteProductCommand(Guid.NewGuid());

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }
    }
}