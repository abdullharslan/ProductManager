// ProductManager.API/Controllers/ProductController.cs

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ProductManager.Application.Interfaces;
using ProductManager.Core.DTOs;

namespace ProductManager.API.Controllers;

/*
 * ProductController, ürün yönetimi için HTTP endpoint'lerini sağlar.
 * RESTful prensiplerine uygun olarak tasarlanmıştır.
 *
 * Endpoint'ler:
 * - GET /api/products: Tüm ürünleri listeler
 * - GET /api/products/{id}: Belirli bir ürünü getirir
 * - POST /api/products: Yeni ürün oluşturur
 * - PUT /api/products/{id}: Mevcut ürünü günceller
 * - DELETE /api/products/{id}: Ürünü soft-delete yapar
 * - GET /api/products/active: Aktif ürünleri listeler
 * - GET /api/products/search: İsme göre ürün arar
 */
[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllProducts();
        return Ok(products);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var product = await _productService.GetProductById(id);
            return Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product not found: {Id}", id);
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto productDto)
    {
        var createdProduct = await _productService.CreateProduct(productDto);
        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto productDto)
    {
        await _productService.UpdateProduct(id, productDto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _productService.DeleteProduct(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product not found for delete: {Id}", id);
            return NotFound(ex.Message);
        }
    }

    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive()
    {
        var products = await _productService.GetActiveProducts();
        return Ok(products);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        try
        {
            var products = await _productService.SearchProducts(name);
            return Ok(products);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid search term provided");
            return BadRequest(ex.Message);
        }
    }
}