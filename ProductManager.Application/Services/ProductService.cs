// ProductManager.Application/Services/ProductService.cs

using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using ProductManager.Application.Exceptions;
using ProductManager.Application.Interfaces;
using ProductManager.Core.DTOs;
using ProductManager.Core.Entities;
using ProductManager.Core.Interfaces;

namespace ProductManager.Application.Services;

/*
 * ProductService sınıfı, ürünlerle ilgili tüm iş mantığını yönetir.
 * Repository pattern üzerinden veritabanı işlemlerini gerçekleştirir ve
 * AutoMapper ile entity-DTO dönüşümlerini sağlar.
 *
 * Bu sınıf:
 * - Ürün verilerinin validasyonunu yapar
 * - İş kurallarını uygular
 * - Entity-DTO dönüşümlerini gerçekleştirir
 * - Hata yönetimini sağlar
 */
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateProductDto> _validator;

    public ProductService(
        IProductRepository productRepository, 
        IMapper mapper,
        IValidator<CreateProductDto> validator)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _validator = validator;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProducts()
    {
        var products = await _productRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> GetProductById(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        return _mapper.Map<ProductDto>(product);
    }

    // ProductManager.Application/Services/ProductService.cs içindeki CreateProduct ve UpdateProduct metodları
    public async Task<ProductDto> CreateProduct(CreateProductDto productDto)
    {
        var validationResult = await _validator.ValidateAsync(productDto);
    
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            throw new CustomValidationException(errors);
        }

        var product = _mapper.Map<Product>(productDto);
        var createdProduct = await _productRepository.AddAsync(product);
        return _mapper.Map<ProductDto>(createdProduct);
    }

    public async Task UpdateProduct(int id, CreateProductDto productDto)
    {
        var validationResult = await _validator.ValidateAsync(productDto);
    
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();
            throw new CustomValidationException(errors);
        }

        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        _mapper.Map(productDto, existingProduct);
        await _productRepository.UpdateAsync(existingProduct);
    }

    public async Task DeleteProduct(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        await _productRepository.DeleteAsync(product);
    }

    public async Task<IEnumerable<ProductDto>> GetActiveProducts()
    {
        var products = await _productRepository.GetActiveProductsAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> SearchProducts(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Search term cannot be empty.");

        var products = await _productRepository.GetProductsByNameAsync(name);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}