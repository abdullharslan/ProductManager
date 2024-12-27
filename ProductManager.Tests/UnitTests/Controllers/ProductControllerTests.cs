// ProductManager.Tests/UnitTests/Controllers/ProductControllerTests.cs

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManager.API.Controllers;
using ProductManager.Application.Exceptions;
using ProductManager.Application.Interfaces;
using ProductManager.Core.DTOs;

namespace ProductManager.Tests.UnitTests.Controllers;

/*
* ProductControllerTests sınıfı, ProductController için unit testleri içerir.
* Bu test sınıfı, API katmanındaki endpoint'lerin doğru çalıştığını kontrol eder.
*
* Test Kapsamı:
* - HTTP Get (tüm ürünler, tek ürün, aktif ürünler, arama)
* - HTTP Post (yeni ürün oluşturma)
* - HTTP Put (ürün güncelleme)
* - HTTP Delete (ürün silme)
* - Başarılı ve başarısız senaryolar
* - HTTP durum kodlarının doğruluğu
* 
* Her test vakası için beklenen HTTP yanıtları ve durum kodları kontrol edilir.
*/
public class ProductControllerTests
{
   private readonly Mock<IProductService> _mockService;
   private readonly Mock<ILogger<ProductController>> _mockLogger;
   private readonly ProductController _controller;

   public ProductControllerTests()
   {
       _mockService = new Mock<IProductService>();
       _mockLogger = new Mock<ILogger<ProductController>>();
       _controller = new ProductController(_mockService.Object, _mockLogger.Object);
   }

   [Fact]
   public async Task GetAll_ShouldReturnOkResult_WithProducts()
   {
       // Arrange
       var products = new List<ProductDto>
       {
           new() { Id = 1, Name = "Test Product 1", Price = 100 },
           new() { Id = 2, Name = "Test Product 2", Price = 200 }
       };

       _mockService.Setup(service => service.GetAllProducts())
           .ReturnsAsync(products);

       // Act
       var result = await _controller.GetAll();

       // Assert
       var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
       var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
       returnedProducts.Should().BeEquivalentTo(products);
       okResult.StatusCode.Should().Be(200);
   }

   [Fact]
   public async Task GetById_WithValidId_ShouldReturnOkResult()
   {
       // Arrange
       var productId = 1;
       var product = new ProductDto 
       { 
           Id = productId, 
           Name = "Test Product",
           Price = 100
       };

       _mockService.Setup(service => service.GetProductById(productId))
           .ReturnsAsync(product);

       // Act
       var result = await _controller.GetById(productId);

       // Assert
       var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
       var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;
       returnedProduct.Should().BeEquivalentTo(product);
       okResult.StatusCode.Should().Be(200);
   }

   [Fact]
   public async Task GetById_WithInvalidId_ShouldReturnNotFound()
   {
       // Arrange
       var invalidId = 999;
       _mockService.Setup(service => service.GetProductById(invalidId))
           .ThrowsAsync(new KeyNotFoundException($"Product with ID {invalidId} not found."));

       // Act
       var result = await _controller.GetById(invalidId);

       // Assert
       var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
       notFoundResult.StatusCode.Should().Be(404);
       notFoundResult.Value.Should().Be($"Product with ID {invalidId} not found.");
   }

   [Fact]
   public async Task Create_WithValidData_ShouldReturnCreatedResult()
   {
       // Arrange
       var createDto = new CreateProductDto
       {
           Name = "New Product",
           Price = 150,
           Description = "Test Description",
           StockQuantity = 10
       };

       var createdProduct = new ProductDto
       {
           Id = 1,
           Name = "New Product",
           Price = 150,
           Description = "Test Description",
           StockQuantity = 10,
           IsActive = true
       };

       _mockService.Setup(service => service.CreateProduct(createDto))
           .ReturnsAsync(createdProduct);

       // Act
       var result = await _controller.Create(createDto);

       // Assert
       var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
       createdResult.StatusCode.Should().Be(201);
       createdResult.ActionName.Should().Be(nameof(ProductController.GetById));
       createdResult.RouteValues["id"].Should().Be(createdProduct.Id);
       
       var returnedProduct = createdResult.Value.Should().BeOfType<ProductDto>().Subject;
       returnedProduct.Should().BeEquivalentTo(createdProduct);
   }

   [Fact]
   public async Task Create_WithInvalidData_ShouldReturnBadRequest()
   {
       // Arrange
       var invalidDto = new CreateProductDto
       {
           Name = "", // Invalid: Empty name
           Price = -1 // Invalid: Negative price
       };

       var validationErrors = new List<string>
       {
           "Name is required",
           "Price must be greater than zero"
       };

       _mockService.Setup(service => service.CreateProduct(invalidDto))
           .ThrowsAsync(new CustomValidationException(validationErrors));

       // Act
       var result = await _controller.Create(invalidDto);

       // Assert
       var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
       badRequestResult.StatusCode.Should().Be(400);
   }

   [Fact]
   public async Task Update_WithValidData_ShouldReturnNoContent()
   {
       // Arrange
       var productId = 1;
       var updateDto = new CreateProductDto
       {
           Name = "Updated Product",
           Price = 200,
           Description = "Updated Description",
           StockQuantity = 15
       };

       // Act
       var result = await _controller.Update(productId, updateDto);

       // Assert
       result.Should().BeOfType<NoContentResult>();
       var noContentResult = result as NoContentResult;
       noContentResult.StatusCode.Should().Be(204);
   }

   [Fact]
   public async Task Update_WithNonExistingId_ShouldReturnNotFound()
   {
       // Arrange
       var nonExistingId = 999;
       var updateDto = new CreateProductDto
       {
           Name = "Updated Product",
           Price = 200
       };

       _mockService.Setup(service => service.UpdateProduct(nonExistingId, updateDto))
           .ThrowsAsync(new KeyNotFoundException($"Product with ID {nonExistingId} not found."));

       // Act
       var result = await _controller.Update(nonExistingId, updateDto);

       // Assert
       var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
       notFoundResult.StatusCode.Should().Be(404);
       notFoundResult.Value.Should().Be($"Product with ID {nonExistingId} not found.");
   }

   [Fact]
   public async Task Delete_WithValidId_ShouldReturnNoContent()
   {
       // Arrange
       var productId = 1;

       // Act
       var result = await _controller.Delete(productId);

       // Assert
       result.Should().BeOfType<NoContentResult>();
       var noContentResult = result as NoContentResult;
       noContentResult.StatusCode.Should().Be(204);
   }

   [Fact]
   public async Task GetActive_ShouldReturnOkResult_WithActiveProducts()
   {
       // Arrange
       var activeProducts = new List<ProductDto>
       {
           new() { Id = 1, Name = "Active Product 1", IsActive = true },
           new() { Id = 2, Name = "Active Product 2", IsActive = true }
       };

       _mockService.Setup(service => service.GetActiveProducts())
           .ReturnsAsync(activeProducts);

       // Act
       var result = await _controller.GetActive();

       // Assert
       var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
       var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
       returnedProducts.Should().BeEquivalentTo(activeProducts);
       returnedProducts.Should().AllSatisfy(p => p.IsActive.Should().BeTrue());
       okResult.StatusCode.Should().Be(200);
   }

   [Fact]
   public async Task Search_WithValidTerm_ShouldReturnOkResult()
   {
       // Arrange
       var searchTerm = "Test";
       var matchingProducts = new List<ProductDto>
       {
           new() { Id = 1, Name = "Test Product 1" },
           new() { Id = 2, Name = "Test Product 2" }
       };

       _mockService.Setup(service => service.SearchProducts(searchTerm))
           .ReturnsAsync(matchingProducts);

       // Act
       var result = await _controller.Search(searchTerm);

       // Assert
       var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
       var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductDto>>().Subject;
       returnedProducts.Should().BeEquivalentTo(matchingProducts);
       okResult.StatusCode.Should().Be(200);
   }

   [Fact]
   public async Task Search_WithEmptyTerm_ShouldReturnBadRequest()
   {
       // Arrange
       var emptyTerm = "";
       _mockService.Setup(service => service.SearchProducts(emptyTerm))
           .ThrowsAsync(new ArgumentException("Search term cannot be empty."));

       // Act
       var result = await _controller.Search(emptyTerm);

       // Assert
       var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
       badRequestResult.StatusCode.Should().Be(400);
       badRequestResult.Value.Should().Be("Search term cannot be empty.");
   }
}