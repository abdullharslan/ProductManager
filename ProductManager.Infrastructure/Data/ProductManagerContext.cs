// ProductManager.Infrastructure/Data/ProductManagerContext.cs

using Microsoft.EntityFrameworkCore;
using ProductManager.Core.Entities;
using ProductManager.Infrastructure.Data.Configurations;

namespace ProductManager.Infrastructure.Data;

/*
 * ProductManagerContext sınıfı, Entity Framework Core için veritabanı bağlam sınıfıdır.
 * Bu sınıf, veritabanı tablolarını ve bunların yapılandırmalarını yönetir.
 *
 * DbSet<Product>: Ürünler tablosunu temsil eder.
 * OnModelCreating: Entity yapılandırmalarının tanımlandığı metod.
 */
public class ProductManagerContext : DbContext
{
    public ProductManagerContext(DbContextOptions<ProductManagerContext> options) 
        : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
    }
}