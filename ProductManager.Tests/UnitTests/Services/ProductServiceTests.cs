// ProductManager.Tests/UnitTests/Services/ProductServiceTests.cs

using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using ProductManager.Application.Exceptions;
using ProductManager.Application.Services;
using ProductManager.Core.DTOs;
using ProductManager.Core.Entities;
using ProductManager.Core.Interfaces;

namespace ProductManager.Tests.UnitTests.Services;

/*
* ProductServiceTests sınıfı, ProductService için unit testleri içerir.
* Bu test sınıfı, servis katmanındaki iş mantığının doğru çalıştığını kontrol eder.
*
* Test Kapsamı:
* - Ürün listeleme
* - Ürün oluşturma
* - Ürün güncelleme
* - Ürün silme
* - Aktif ürünleri listeleme
* - Ürün arama
* - Hata durumları
*
* Her test metodu Arrange-Act-Assert (AAA) pattern'ini takip eder.
*/
public class ProductServiceTests
{
   private readonly Mock<IProductRepository> _mockRepo;
   private readonly Mock<IMapper> _mockMapper;
   private readonly Mock<IValidator<CreateProductDto>> _mockValidator;
   private readonly ProductService _service;

   public ProductServiceTests()
   {
       _mockRepo = new Mock<IProductRepository>();
       _mockMapper = new Mock<IMapper>();
       _mockValidator = new Mock<IValidator<CreateProductDto>>();
       _service = new ProductService(_mockRepo.Object, _mockMapper.Object, _mockValidator.Object);
   }

   [Fact]
   public async Task GetAllProducts_ShouldReturnProductList()
   {
       // Arrange
       var productList = new List<Product>
       {
           new() { Id = 1, Name = "Test Product 1", Price = 100, IsActive = true },
           new() { Id = 2, Name = "Test Product 2", Price = 200, IsActive = true }
       };

       var productDtoList = new List<ProductDto>
       {
           new() { Id = 1, Name = "Test Product 1", Price = 100, IsActive = true },
           new() { Id = 2, Name = "Test Product 2", Price = 200, IsActive = true }
       };

       _mockRepo.Setup(repo => repo.GetAllAsync())
           .ReturnsAsync(productList);

       _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(productList))
           .Returns(productDtoList);

       // Act
       var result = await _service.GetAllProducts();

       // Assert
       result.Should().NotBeNull();
       result.Should().BeEquivalentTo(productDtoList);
       _mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
   }

   [Fact]
   public async Task GetProductById_WithValidId_ShouldReturnProduct()
   {
       // Arrange
       var productId = 1;
       var product = new Product 
       { 
           Id = productId, 
           Name = "Test Product", 
           Price = 100,
           Description = "Test Description",
           StockQuantity = 10,
           IsActive = true 
       };

       var productDto = new ProductDto 
       { 
           Id = productId, 
           Name = "Test Product", 
           Price = 100,
           Description = "Test Description",
           StockQuantity = 10,
           IsActive = true 
       };

       _mockRepo.Setup(repo => repo.GetByIdAsync(productId))
           .ReturnsAsync(product);

       _mockMapper.Setup(mapper => mapper.Map<ProductDto>(product))
           .Returns(productDto);

       // Act
       var result = await _service.GetProductById(productId);

       // Assert
       result.Should().NotBeNull();
       result.Should().BeEquivalentTo(productDto);
       _mockRepo.Verify(repo => repo.GetByIdAsync(productId), Times.Once);
   }

   [Fact]
   public async Task GetProductById_WithInvalidId_ShouldThrowKeyNotFoundException()
   {
       // Arrange
       var invalidId = 999;
       _mockRepo.Setup(repo => repo.GetByIdAsync(invalidId))
           .ReturnsAsync((Product)null);

       // Act & Assert
       await _service.Invoking(s => s.GetProductById(invalidId))
           .Should().ThrowAsync<KeyNotFoundException>()
           .WithMessage($"Product with ID {invalidId} not found.");
   }

   [Fact]
   public async Task CreateProduct_WithValidData_ShouldReturnCreatedProduct()
   {
       // Arrange
       var createDto = new CreateProductDto
       {
           Name = "New Product",
           Price = 150,
           Description = "Test Description",
           StockQuantity = 10
       };

       var product = new Product
       {
           Id = 1,
           Name = "New Product",
           Price = 150,
           Description = "Test Description",
           StockQuantity = 10,
           IsActive = true,
           CreatedDate = DateTime.UtcNow
       };

       var productDto = new ProductDto
       {
           Id = 1,
           Name = "New Product",
           Price = 150,
           Description = "Test Description",
           StockQuantity = 10,
           IsActive = true,
           CreatedDate = product.CreatedDate
       };

       _mockValidator.Setup(v => v.ValidateAsync(createDto, default))
           .ReturnsAsync(new FluentValidation.Results.ValidationResult());

       _mockMapper.Setup(mapper => mapper.Map<Product>(createDto))
           .Returns(product);

       _mockRepo.Setup(repo => repo.AddAsync(product))
           .ReturnsAsync(product);

       _mockMapper.Setup(mapper => mapper.Map<ProductDto>(product))
           .Returns(productDto);

       // Act
       var result = await _service.CreateProduct(createDto);

       // Assert
       result.Should().NotBeNull();
       result.Should().BeEquivalentTo(productDto);
       _mockRepo.Verify(repo => repo.AddAsync(product), Times.Once);
   }

   [Fact]
   public async Task CreateProduct_WithInvalidData_ShouldThrowValidationException()
   {
       // Arrange
       var invalidDto = new CreateProductDto
       {
           Name = "", // Invalid: Empty name
           Price = -1, // Invalid: Negative price
           StockQuantity = -5 // Invalid: Negative stock
       };

       var validationFailures = new List<ValidationFailure>
       {
           new("Name", "Name is required"),
           new("Price", "Price must be greater than zero"),
           new("StockQuantity", "Stock quantity cannot be negative")
       };

       var validationResult = new FluentValidation.Results.ValidationResult(validationFailures);

       _mockValidator.Setup(v => v.ValidateAsync(invalidDto, default))
           .ReturnsAsync(validationResult);

       // Act & Assert
       await _service.Invoking(s => s.CreateProduct(invalidDto))
           .Should().ThrowAsync<CustomValidationException>()
           .WithMessage("One or more validation errors occurred.");
   }

   [Fact]
   public async Task UpdateProduct_WithValidData_ShouldSucceed()
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

       var existingProduct = new Product
       {
           Id = productId,
           Name = "Old Product",
           Price = 150,
           Description = "Old Description",
           StockQuantity = 10,
           IsActive = true,
           CreatedDate = DateTime.UtcNow.AddDays(-1)
       };

       _mockValidator.Setup(v => v.ValidateAsync(updateDto, default))
           .ReturnsAsync(new FluentValidation.Results.ValidationResult());

       _mockRepo.Setup(repo => repo.GetByIdAsync(productId))
           .ReturnsAsync(existingProduct);

       // Act
       await _service.UpdateProduct(productId, updateDto);

       // Assert
       _mockRepo.Verify(repo => repo.UpdateAsync(It.Is<Product>(p => 
           p.Id == productId && 
           p.Name == updateDto.Name && 
           p.Price == updateDto.Price)), 
           Times.Once);
   }

   [Fact]
   public async Task DeleteProduct_WithExistingId_ShouldSucceed()
   {
       // Arrange
       var productId = 1;
       var existingProduct = new Product { Id = productId, Name = "Test Product", IsActive = true };

       _mockRepo.Setup(repo => repo.GetByIdAsync(productId))
           .ReturnsAsync(existingProduct);

       // Act
       await _service.DeleteProduct(productId);

       // Assert
       _mockRepo.Verify(repo => repo.DeleteAsync(existingProduct), Times.Once);
   }

   [Fact]
   public async Task DeleteProduct_WithNonExistingId_ShouldThrowKeyNotFoundException()
   {
       // Arrange
       var nonExistingId = 999;
       _mockRepo.Setup(repo => repo.GetByIdAsync(nonExistingId))
           .ReturnsAsync((Product)null);

       // Act & Assert
       await _service.Invoking(s => s.DeleteProduct(nonExistingId))
           .Should().ThrowAsync<KeyNotFoundException>()
           .WithMessage($"Product with ID {nonExistingId} not found.");
   }

   [Fact]
   public async Task GetActiveProducts_ShouldReturnOnlyActiveProducts()
   {
       // Arrange
       var productList = new List<Product>
       {
           new() { Id = 1, Name = "Active Product 1", IsActive = true },
           new() { Id = 2, Name = "Active Product 2", IsActive = true }
       };

       var productDtoList = new List<ProductDto>
       {
           new() { Id = 1, Name = "Active Product 1", IsActive = true },
           new() { Id = 2, Name = "Active Product 2", IsActive = true }
       };

       _mockRepo.Setup(repo => repo.GetActiveProductsAsync())
           .ReturnsAsync(productList);

       _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(productList))
           .Returns(productDtoList);

       // Act
       var result = await _service.GetActiveProducts();

       // Assert
       result.Should().NotBeNull();
       result.Should().BeEquivalentTo(productDtoList);
       result.Should().AllSatisfy(p => p.IsActive.Should().BeTrue());
       _mockRepo.Verify(repo => repo.GetActiveProductsAsync(), Times.Once);
   }

   [Fact]
   public async Task SearchProducts_WithValidName_ShouldReturnMatchingProducts()
   {
       // Arrange
       var searchTerm = "Test";
       var productList = new List<Product>
       {
           new() { Id = 1, Name = "Test Product 1", IsActive = true },
           new() { Id = 2, Name = "Test Product 2", IsActive = true }
       };

       var productDtoList = new List<ProductDto>
       {
           new() { Id = 1, Name = "Test Product 1", IsActive = true },
           new() { Id = 2, Name = "Test Product 2", IsActive = true }
       };

       _mockRepo.Setup(repo => repo.GetProductsByNameAsync(searchTerm))
           .ReturnsAsync(productList);

       _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(productList))
           .Returns(productDtoList);

       // Act
       var result = await _service.SearchProducts(searchTerm);

       // Assert
       result.Should().NotBeNull();
       result.Should().BeEquivalentTo(productDtoList);
       result.Should().AllSatisfy(p => p.Name.Should().Contain(searchTerm));
       _mockRepo.Verify(repo => repo.GetProductsByNameAsync(searchTerm), Times.Once);
   }

   [Fact]
   public async Task SearchProducts_WithEmptyName_ShouldThrowArgumentException()
   {
       // Arrange
       string emptySearch = "";

       // Act & Assert
       await _service.Invoking(s => s.SearchProducts(emptySearch))
           .Should().ThrowAsync<ArgumentException>()
           .WithMessage("Search term cannot be empty.");
   }
}