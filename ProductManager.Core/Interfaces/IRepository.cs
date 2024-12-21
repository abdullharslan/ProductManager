// ProductManager.Core/Interfaces/IRepository.cs

namespace ProductManager.Core.Interfaces;

/*
 * Generic Repository interface'i, temel CRUD operasyonlarını tanımlar.
 * Bu interface, farklı entity tipleri için ortak veritabanı işlemlerini standartlaştırır.
 *
 * Generic Tip Parametresi:
 * - T: Entity tipi (class kısıtlaması ile)
 *
 * Metodlar:
 * - GetAllAsync: Tüm kayıtları asenkron olarak getirir.
 * - GetByIdAsync: ID'ye göre tek bir kaydı asenkron olarak getirir.
 * - AddAsync: Yeni bir kayıt ekler.
 * - UpdateAsync: Mevcut bir kaydı günceller.
 * - DeleteAsync: Bir kaydı siler.
 */
public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}