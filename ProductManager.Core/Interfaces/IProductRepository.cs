// ProductManager.Core/Interfaces/IProductRepository.cs

using ProductManager.Core.Entities;

namespace ProductManager.Core.Interfaces;

/*
 * IProductRepository interface'i, Product entity'sine özel repository operasyonlarını tanımlar.
 * Bu interface, genel repository fonksiyonlarına ek olarak ürünlere özel sorguları içerir.
 *
 * Özel Metodlar:
 * - GetActiveProductsAsync: Sadece aktif ürünleri getirir.
 * - GetProductsByNameAsync: İsme göre ürün araması yapar.
 *
 * Not: Bu interface IRepository<Product>'dan kalıtım alır ve onun tüm metodlarını içerir.
 */
public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<IEnumerable<Product>> GetProductsByNameAsync(string name);
}