using System.Net;
using System.Net.Http.Json;
using Catalog.API.Products.CreateProduct;
using Catalog.API.Products.GetProductByCategory;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace Catalog.API.Tests.Integration.Products
{
    public class GetProductByCategoryTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public GetProductByCategoryTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProductByCategory_ShouldReturnProducts_WhenCategoryExists()
        {
            // Arrange: First create a product in the "Books" category
            var createRequest = new CreateProductRequest(
                Name: "Integration Test Book",
                Category: new List<string> { "Books" },
                Description: "A test book for category filter",
                ImageFile: "book.jpg",
                Price: 49.99m
            );

            var createResponse = await _client.PostAsJsonAsync("/products", createRequest);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // Act: Now query products by category
            var getResponse = await _client.GetAsync("/products/category/Books");

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await getResponse.Content.ReadFromJsonAsync<GetProductByCategoryResponse>();
            responseBody.Should().NotBeNull();
            responseBody!.Products.Should().NotBeEmpty();
            responseBody.Products.Should().Contain(p => p.Category.Contains("Books"));
        }

        [Fact]
        public async Task GetProductByCategory_ShouldReturnEmpty_WhenCategoryDoesNotExist()
        {
            // Act
            var getResponse = await _client.GetAsync("/products/category/NonExistentCategory");

            // Assert
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseBody = await getResponse.Content.ReadFromJsonAsync<GetProductByCategoryResponse>();
            responseBody.Should().NotBeNull();
            responseBody!.Products.Should().BeEmpty();
        }
    }
}