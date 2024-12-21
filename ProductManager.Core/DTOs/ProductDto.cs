// ProductManager.Core/DTOs/ProductDto.cs
namespace ProductManager.Core.DTOs;

/*
 * ProductDto sınıfı, Product entity'sinin veri transfer nesnesidir.
 * Bu DTO, kullanıcı arayüzü ve servis katmanı arasında ürün verilerinin taşınmasını sağlar.
 *
 * Bu sınıf, Product entity'sinin tüm özelliklerini içerir ve
 * presentation layer'da kullanılmak üzere tasarlanmıştır.
 */
public class ProductDto
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