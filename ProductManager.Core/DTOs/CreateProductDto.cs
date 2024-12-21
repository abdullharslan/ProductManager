// ProductManager.Core/DTOs/CreateProductDto.cs
namespace ProductManager.Core.DTOs;

/*
 * CreateProductDto sınıfı, yeni bir ürün oluşturulurken kullanılan DTO'dur.
 * Bu DTO, ürün oluşturma işlemi için gerekli minimum veri setini içerir.
 *
 * Özellikler:
 * - Name: Yeni ürünün adı
 * - Description: Yeni ürünün açıklaması
 * - Price: Yeni ürünün fiyatı
 * - StockQuantity: Yeni ürünün başlangıç stok miktarı
 *
 * Not: Id, CreatedDate gibi sistem tarafından otomatik atanan alanlar bu DTO'da yer almaz.
 */
public class CreateProductDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}