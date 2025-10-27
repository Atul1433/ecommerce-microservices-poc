using Catalog.API.Products.GetProductByCategory;
using Marten.Linq;
using MockQueryable.Moq;
using Moq;

namespace Catalog.API.Tests.Unit.Products.GetProductByCategory
{
    public class GetProductByCategoryHandlerTests
    {
        private readonly Mock<IDocumentSession> _mockSession;
        private readonly GetProductByCategoryQueryHandler _handler;
        private readonly Faker<Product> _productFaker;

        public GetProductByCategoryHandlerTests()
        {
            
            _mockSession = new Mock<IDocumentSession>();
            _handler = new GetProductByCategoryQueryHandler(_mockSession.Object);

            // Setup Bogus to generate fake products
            _productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.ImageFile, f => f.Image.PicsumUrl())
                .RuleFor(p => p.Price, f => f.Random.Decimal(100, 1000));
                // Note: Category rule is removed here
                // It's better to set it per-test for explicit control
        }

        [Fact]
        public async Task Handle_Should_Return_Only_Products_Matching_Category()
        {
            // Arrange
            
            var category = "Books";
            var query = new GetProductByCategoryQuery(category);

            // 1. Create products that *should* match
            var matchingProducts = _productFaker.Generate(3);
            matchingProducts.ForEach(p => p.Category = new List<string> { "Books", "Stationery" });

            // 2. Create products that *should not* match
            var nonMatchingProducts = _productFaker.Generate(2);
            nonMatchingProducts.ForEach(p => p.Category = new List<string> { "Electronics", "Computers" });

            // 3. Combine them into the "database" source
            var allProducts = new List<Product>();
            allProducts.AddRange(matchingProducts);
            allProducts.AddRange(nonMatchingProducts);

            // 4. Create a mock IQueryable from the in-memory list.
            //    This mock will correctly execute LINQ methods like .Where() in memory.
             var mockQueryable = allProducts.AsQueryable().BuildMock();
            

            // 5. Marten's s.Query<T>() returns IMartenQueryable<T>, which inherits IQueryable<T>.
            //    We must mock IMartenQueryable<T> and forward the IQueryable implementation
            //    to our mockQueryable object. This is what allows .Where() to work.
            var mockMartenQueryable = new Mock<IMartenQueryable<Product>>();

            // Forward IQueryable methods to the mockQueryable.Object
            mockMartenQueryable.As<IQueryable<Product>>().Setup(x => x.Provider).Returns(mockQueryable.Object.Provider);
            mockMartenQueryable.As<IQueryable<Product>>().Setup(x => x.Expression).Returns(mockQueryable.Object.Expression);
            mockMartenQueryable.As<IQueryable<Product>>().Setup(x => x.ElementType).Returns(mockQueryable.Object.ElementType);
            mockMartenQueryable.As<IQueryable<Product>>().Setup(x => x.GetEnumerator()).Returns(mockQueryable.Object.GetEnumerator());

            // 6. Setup the session to return our mock
            //    Now, when the handler calls session.Query<Product>().Where(...),
            //    it will execute the .Where() against our in-memory list.
            _mockSession.Setup(s => s.Query<Product>())
                        .Returns(mockMartenQueryable.Object);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().NotBeNull();
            
            // We expect *only* the 3 matching products
            result.Products.Should().HaveCount(3);
            
            // Verify that *all* returned products contain the category
            result.Products.Should().OnlyContain(p => p.Category.Contains(category));
            
            // A stronger assertion: the returned list should be equivalent to our expected list
            result.Products.Should().BeEquivalentTo(matchingProducts);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_Products_Match_Category()
        {
            // Arrange
            var category = "NonExistentCategory";
            var query = new GetProductByCategoryQuery(category);

            // Create products that *do not* match
            var nonMatchingProducts = _productFaker.Generate(5);
            nonMatchingProducts.ForEach(p => p.Category = new List<string> { "Electronics", "Computers" });

            // Setup the mock queryable (same as above)
            var mockQueryable = nonMatchingProducts.AsQueryable().BuildMock();
            var mockMartenQueryable = new Mock<IMartenQueryable<Product>>();

            mockMartenQueryable.As<IQueryable<Product>>().Setup(x => x.Provider).Returns(mockQueryable.Object.Provider);
            mockMartenQueryable.As<IQueryable<Product>>().Setup(x => x.Expression).Returns(mockQueryable.Object.Expression);
            mockMartenQueryable.As<IQueryable<Product>>().Setup(x => x.ElementType).Returns(mockQueryable.Object.ElementType);
            mockMartenQueryable.As<IQueryable<Product>>().Setup(x => x.GetEnumerator()).Returns(mockQueryable.Object.GetEnumerator());

            _mockSession.Setup(s => s.Query<Product>())
                        .Returns(mockMartenQueryable.Object);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Products.Should().NotBeNull();
            result.Products.Should().BeEmpty(); // Expect an empty list
        }
    }
}