// ProductManager.Infrastructure/Repositories/ProductRepository.cs

using Microsoft.EntityFrameworkCore;
using ProductManager.Core.Entities;
using ProductManager.Core.Interfaces;
using ProductManager.Infrastructure.Data;

namespace ProductManager.Infrastructure.Repositories;

/*
 * ProductRepository sınıfı, IProductRepository interface'ini implemente eder ve
 * ürünlerle ilgili tüm veritabanı işlemlerini gerçekleştirir.
 *
 * Bu sınıf:
 * - Temel CRUD operasyonlarını gerçekleştirir
 * - Özel ürün sorgularını yönetir
 * - Entity Framework Core kullanarak veritabanı işlemlerini yapar
 */
public class ProductRepository : IProductRepository
{
    private readonly ProductManagerContext _context;

    public ProductRepository(ProductManagerContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<Product> AddAsync(Product entity)
    {
        entity.CreatedDate = DateTime.UtcNow;
        entity.IsActive = true;
        
        await _context.Products.AddAsync(entity);
        await _context.SaveChangesAsync();
        
        return entity;
    }

    public async Task UpdateAsync(Product entity)
    {
        entity.UpdatedDate = DateTime.UtcNow;
        _context.Products.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product entity)
    {
        entity.IsActive = false;
        entity.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsActive)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByNameAsync(string name)
    {
        return await _context.Products
            .Where(p => p.Name.Contains(name) && p.IsActive)
            .ToListAsync();
    }
}