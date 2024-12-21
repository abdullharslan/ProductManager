// ProductManager.Application/Interfaces/IProductService.cs

using ProductManager.Core.DTOs;

namespace ProductManager.Application.Interfaces;

/*
 * IProductService, uygulama katmanındaki ürün işlemlerini tanımlayan service interface'idir.
 * Bu interface, iş mantığı operasyonlarını ve DTO dönüşümlerini yönetir.
 *
 * Metodlar:
 * - GetAllProducts: Tüm ürünleri DTO formatında getirir
 * - GetProductById: ID'ye göre ürün detayını DTO formatında getirir
 * - CreateProduct: Yeni ürün oluşturur
 * - UpdateProduct: Mevcut ürünü günceller
 * - DeleteProduct: Ürünü soft-delete yapar
 * - GetActiveProducts: Aktif ürünleri listeler
 * - SearchProducts: İsme göre ürün araması yapar
 */
public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProducts();
    Task<ProductDto> GetProductById(int id);
    Task<ProductDto> CreateProduct(CreateProductDto productDto);
    Task UpdateProduct(int id, CreateProductDto productDto);
    Task DeleteProduct(int id);
    Task<IEnumerable<ProductDto>> GetActiveProducts();
    Task<IEnumerable<ProductDto>> SearchProducts(string name);
}