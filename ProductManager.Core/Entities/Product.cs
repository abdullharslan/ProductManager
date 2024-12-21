// ProductManager.Core/Entities/Product.cs

namespace ProductManager.Core.Entities;

/*
 * Product sınıfı, ürünlere ilişkin verilerin tanımlandığı temel bir domain sınıfıdır.
 * Bu sınıf, uygulamanın iş mantığında kullanılan ürün bilgilerini temsil eder.
 *
 * Özellikler:
 * - Id: Ürünün benzersiz kimlik numarası.
 * - Name: Ürünün adı.
 * - Description: Ürünün açıklaması.
 * - Price: Ürünün fiyatı.
 * - StockQuantity: Ürünün stok miktarı.
 * - CreatedDate: Ürünün oluşturulma tarihi.
 * - UpdatedDate: Ürünün son güncellenme tarihi (isteğe bağlı).
 * - IsActive: Ürünün aktif olup olmadığını belirtir.
 */
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public bool IsActive { get; set; }
}