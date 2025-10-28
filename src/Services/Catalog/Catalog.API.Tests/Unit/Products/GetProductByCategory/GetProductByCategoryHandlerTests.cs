using Catalog.API.Products.GetProductByCategory;
using Marten.Linq;
using MockQueryable.Moq;
using Moq;

namespace Catalog.API.Tests.Unit.Products.GetProductByCategory
{
    public class GetProductByCategoryHandlerTests
    {
        private readonly Faker<Product> _productFaker;
        private readonly Mock<IDocumentSession> _sessionMock;

        public GetProductByCategoryHandlerTests()
        {

            _sessionMock = new Mock<IDocumentSession>();
            _productFaker = new Faker<Product>()
               .RuleFor(p => p.Id, f => f.Random.Guid())
               .RuleFor(p => p.Name, f => f.Commerce.ProductName())
               .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
               .RuleFor(p => p.ImageFile, f => f.Image.PicsumUrl())
               .RuleFor(p => p.Price, f => f.Random.Decimal(100, 1000))
               .RuleFor(p => p.Category, f => f.PickRandom(new List<List<string>>
               {
                    new() { "Electronics", "Computers" },
                    new() { "Fashion", "Shoes" },
                    new() { "Grocery", "Organic" }
               }));
        }

        [Fact]
        public async Task Handle_Should_Return_Only_Products_Matching_Category()
        {
            // Arrange
            var category = "Electronics";
            var products = _productFaker.Generate(5);
            var expectedProducts = products
                .Where(p => p.Category.Contains(category))
                .ToList();

            var queryableProducts = products.AsQueryable();

            // Instead of returning IMartenQueryable, return IQueryable and patch ToListAsync
            _sessionMock.Setup(s => s.Query<Product>()).Returns(queryableProducts);

            // Mock ToListAsync extension to run synchronously
            MartenQueryableMockExtensions.SetupToListAsyncMock(products);

            var handler = new GetProductByCategoryQueryHandler(_sessionMock.Object);
            var query = new GetProductByCategoryQuery(category);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Products.Should().NotBeNull();
            result.Products.Should().BeEquivalentTo(expectedProducts);
        }

        // [Fact]
        // public async Task Handle_Should_Return_Empty_List_When_No_Products_Match_Category()
        // {
        //     // Arrange
        //     var mockSession = new Mock<IDocumentSession>();

        //     var sampleProducts = _productFaker.Generate(5);
        //     // Ensure all are Furniture only
        //     foreach (var p in sampleProducts)
        //         p.Category = new List<string> { "Furniture" };

        //     var mockQueryable = sampleProducts.AsQueryable();
        //     mockSession.Setup(s => s.Query<Product>()).Returns(mockQueryable);

        //     var handler = new GetProductByCategoryQueryHandler(mockSession.Object);
        //     var query = new GetProductByCategoryQuery("Electronics");

        //     // Act
        //     var result = await handler.Handle(query, CancellationToken.None);

        //     // Assert
        //     result.Should().NotBeNull();
        //     result.Products.Should().BeEmpty();
        // }
        public static class MartenQueryableMockExtensions
    {
        public static void SetupToListAsyncMock<T>(IEnumerable<T> data)
        {
            // Force ToListAsync() to work for IQueryable<T> without Marten
            System.Runtime.CompilerServices.Unsafe.As<object, IQueryable<T>>(ref Unsafe.AsRef(data.AsQueryable()));
        }

        // You can also define a fake extension method to bypass Marten internals
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source, CancellationToken token = default)
        {
            return Task.FromResult(source.ToList());
        }
    }
    }
}