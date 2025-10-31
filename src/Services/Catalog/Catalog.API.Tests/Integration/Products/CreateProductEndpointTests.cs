using System.Net;
using System.Net.Http.Json;
using Catalog.API.Products.CreateProduct;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace Catalog.API.Tests.Integration.Products
{
    public class CreateProductTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CreateProductTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateProduct_ShouldReturn201Created_AndProductId()
        {
            // Arrange
            var request = new CreateProductRequest(
                Name: "Test Product",
                Category: new List<string> { "Books", "Education" },
                Description: "A sample product for testing",
                ImageFile: "test-image.jpg",
                Price: 99.99m
            );

            // Act
            var response = await _client.PostAsJsonAsync("/products", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var responseBody = await response.Content.ReadFromJsonAsync<CreateProductResponse>();
            responseBody.Should().NotBeNull();
            responseBody!.Id.Should().NotBeEmpty();

            response.Headers.Location!.ToString().Should().Contain($"/products/{responseBody.Id}");
        }

        [Fact]
        public async Task CreateProduct_ShouldReturn400BadRequest_WhenValidationFails()
        {
            // Arrange: Missing Name and Price = 0
            var invalidRequest = new CreateProductRequest(
                Name: "",
                Category: new List<string>(), // Empty
                Description: "Invalid product",
                ImageFile: "",
                Price: 0
            );

            // Act
            var response = await _client.PostAsJsonAsync("/products", invalidRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}