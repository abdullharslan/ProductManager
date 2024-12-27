// ProductManager.Tests/IntegrationTests/ProductApiTests.cs

using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using ProductManager.Core.DTOs;
using ProductManager.Infrastructure.Data;

namespace ProductManager.Tests.IntegrationTests;

/*
* ProductApiTests sınıfı, API'nin entegrasyon testlerini içerir.
* Bu testler API'nin uçtan uca davranışını doğrular.
*
* Test Kapsamı:
* - Gerçek HTTP istekleri
* - Veritabanı işlemleri
* - Tam request/response döngüsü
* - API endpoint'lerinin gerçek davranışları
* 
* WebApplicationFactory kullanılarak in-memory test ortamı oluşturulur.
*/
public class ProductApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing db context registration
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<ProductManagerContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database
                services.AddDbContext<ProductManagerContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Ensure database is created
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ProductManagerContext>();
                db.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedProduct()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "Integration Test Product",
            Description = "Test Description",
            Price = 99.99m,
            StockQuantity = 100
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/products", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProduct.Should().NotBeNull();
        createdProduct.Name.Should().Be(createDto.Name);
        createdProduct.Price.Should().Be(createDto.Price);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnProducts()
    {
        // Arrange - Create test product
        var createDto = new CreateProductDto
        {
            Name = "Test Product",
            Price = 100,
            StockQuantity = 10
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        await _client.PostAsync("/api/products", content);

        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        products.Should().NotBeNull();
        products.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetProductById_WithValidId_ShouldReturnProduct()
    {
        // Arrange - Create test product
        var createDto = new CreateProductDto
        {
            Name = "Test Product Get By Id",
            Price = 150,
            StockQuantity = 20
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _client.PostAsync("/api/products", content);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await _client.GetAsync($"/api/products/{createdProduct.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product.Id.Should().Be(createdProduct.Id);
        product.Name.Should().Be(createDto.Name);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ShouldSucceed()
    {
        // Arrange - Create test product
        var createDto = new CreateProductDto
        {
            Name = "Original Product",
            Price = 100,
            StockQuantity = 10
        };

        var createContent = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _client.PostAsync("/api/products", createContent);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Update data
        var updateDto = new CreateProductDto
        {
            Name = "Updated Product",
            Price = 200,
            StockQuantity = 20
        };

        var updateContent = new StringContent(
            JsonSerializer.Serialize(updateDto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PutAsync($"/api/products/{createdProduct.Id}", updateContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify update
        var getResponse = await _client.GetAsync($"/api/products/{createdProduct.Id}");
        var updatedProduct = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        updatedProduct.Should().NotBeNull();
        updatedProduct.Name.Should().Be(updateDto.Name);
        updatedProduct.Price.Should().Be(updateDto.Price);
    }

    [Fact]
    public async Task DeleteProduct_ShouldMarkProductAsInactive()
    {
        // Arrange - Create test product
        var createDto = new CreateProductDto
        {
            Name = "Product to Delete",
            Price = 100,
            StockQuantity = 10
        };

        var content = new StringContent(
            JsonSerializer.Serialize(createDto),
            Encoding.UTF8,
            "application/json");

        var createResponse = await _client.PostAsync("/api/products", content);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/products/{createdProduct.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify product is inactive
        var getResponse = await _client.GetAsync($"/api/products/{createdProduct.Id}");
        var deletedProduct = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        deletedProduct.Should().NotBeNull();
        deletedProduct.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task SearchProducts_ShouldReturnMatchingProducts()
    {
        // Arrange - Create test products
        var products = new[]
        {
            new CreateProductDto { Name = "Test Search Product 1", Price = 100, StockQuantity = 10 },
            new CreateProductDto { Name = "Test Search Product 2", Price = 200, StockQuantity = 20 },
            new CreateProductDto { Name = "Different Product", Price = 300, StockQuantity = 30 }
        };

        foreach (var product in products)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(product),
                Encoding.UTF8,
                "application/json");

            await _client.PostAsync("/api/products", content);
        }

        // Act
        var response = await _client.GetAsync("/api/products/search?name=Test Search");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var searchResults = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        searchResults.Should().NotBeNull();
        searchResults.Should().HaveCount(2);
        searchResults.Should().AllSatisfy(p => p.Name.Should().Contain("Test Search"));
    }
}