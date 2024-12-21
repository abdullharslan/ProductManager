// ProductManager.Infrastructure/Data/Configurations/ProductConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductManager.Core.Entities;

namespace ProductManager.Infrastructure.Data.Configurations;

/*
 * ProductConfiguration sınıfı, Product entity'sinin veritabanı seviyesindeki
 * yapılandırmalarını içerir.
 *
 * Yapılandırmalar:
 * - Tablo adı ve şema tanımı
 * - Primary key tanımı
 * - Zorunlu alanların belirlenmesi
 * - Maksimum uzunlukların belirlenmesi
 * - Hassas sayısal değerlerin formatının belirlenmesi
 */
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.StockQuantity)
            .IsRequired();
        
        builder.Property(p => p.CreatedDate)
            .IsRequired();
        
        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}