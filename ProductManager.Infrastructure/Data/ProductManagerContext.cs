// ProductManager.Infrastructure/Data/ProductManagerContext.cs

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductManager.Core.Entities;
using ProductManager.Core.Identity.Entities;
using ProductManager.Infrastructure.Data.Configurations;

namespace ProductManager.Infrastructure.Data;

/*
 * ProductManagerContext sınıfı, Entity Framework Core (EF Core) ile veritabanı işlemlerini gerçekleştirmek için
 * kullanılan bir DbContext sınıfıdır.
 * IdentityDbContext sınıfını genişleterek, uygulama kullanıcıları ve rollerinin veritabanı yönetimini sağlar.
 * Ayrıca, ürünlere dair DbSet içeren `Products` özelliği de tanımlar.
 *
 * Sınıfın Metodları:
 * - OnModelCreating: Veritabanı modelini konfigüre eder. Burada, `Product`, `ApplicationUser`, ve `ApplicationRole`
 *   gibi varlıkların konfigürasyonları uygulanır.
 *   - `ApplyConfiguration` metodu ile, her varlık tipi için yapılandırmalar yapılır. Bu konfigürasyonlar,
 *     `ProductConfiguration`, `ApplicationUserConfiguration`, ve `ApplicationRoleConfiguration` gibi sınıflar
 *      aracılığıyla uygulanır.
 */
public class ProductManagerContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ProductManagerContext(DbContextOptions<ProductManagerContext> options) 
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Bu satır Identity tabloları için gerekli

        // Mevcut ürün konfigürasyonu
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        
        // Identity tabloları için özel konfigürasyonlar
        modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationRoleConfiguration());
    }
}