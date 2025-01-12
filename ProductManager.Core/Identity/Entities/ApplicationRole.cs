// ProductManager.Core/Identity/Entities/ApplicationRole.cs

using Microsoft.AspNetCore.Identity;

namespace ProductManager.Core.Identity.Entities;

/*
 * ApplicationRole sınıfı, IdentityRole sınıfından türetilmiştir ve uygulamanın rol tabanlı erişim kontrolünü temsil eder.
 * Bu sınıf, bir kullanıcının rolüne özgü ek özellikler içerir.
 * Bu sınıf, rolün açıklamasını, oluşturulma tarihini ve rolün aktif olup olmadığını takip etmek için kullanılır.
 *
 * Özellikler:
 * - Description: Rolün kısa bir açıklaması.
 * - CreatedDate: Rolün oluşturulma tarihi.
 * - IsActive: Rolün aktif olup olmadığını belirtir.
 */
public class ApplicationRole : IdentityRole
{
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}