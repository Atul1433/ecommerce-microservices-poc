using Bogus;

namespace Catalog.API.Tests.Unit.Products.CreateProduct
{
    public class CreateProductCommandValidatorTests
    {
        private readonly CreateProductCommandValidator _validator = new();
        private readonly Faker<CreateProductCommand> _validCommandFaker = new Faker<CreateProductCommand>()
        .CustomInstantiator(f => new CreateProductCommand(
            f.Commerce.ProductName(),
            new List<string> { f.Commerce.Categories(1)[0] },
            f.Lorem.Sentence(),
            f.Image.PicsumUrl(),
            f.Random.Decimal(10, 1000)
        ));
        [Fact]
        [Trait("Category", "Unit")]
        public void Should_Fail_When_Name_Is_Empty()
        {
            //var command = new CreateProductCommand("", new List<string> { "Books" }, "Desc", "img.jpg", 99.99M);
            var command = _validCommandFaker.Clone().CustomInstantiator(f => new CreateProductCommand(
            "", // invalid name
            new List<string> { f.Commerce.Categories(1)[0] },
            f.Lorem.Sentence(),
            f.Image.PicsumUrl(),
            f.Random.Decimal(10, 1000)
        )).Generate();
            var result = _validator.Validate(command);
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void Should_Pass_When_All_Fields_Are_Valid()
        {
            //var command = new CreateProductCommand("Valid", new List<string> { "Books" }, "Desc", "img.jpg", 99.99M);
            var command = _validCommandFaker.Generate();
            var result = _validator.Validate(command);
            result.IsValid.Should().BeTrue();
        }

    }
}