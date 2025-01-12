// ProductManager.API/Models/ApiResponse.cs

namespace ProductManager.API.Models;

/*
 * ApiResponse, API yanıtlarının temel bir şablonunu sağlar.
 * Farklı türdeki yanıtlar için türetilmiş sınıflar oluşturularak özelleştirilir.
 *
 * Sınıflar:
 * - ApiResponse: Tüm API yanıtları için temel sınıftır. "Status" ve "Message" özelliklerini içerir.
 * - ErrorApiResponse: Hata durumlarındaki yanıtlar için kullanılır. "Status" özelliği "Error" olarak ayarlanır.
 * - ValidationApiResponse: Doğrulama hataları için özel bir sınıftır. Ek olarak "Errors" özelliği ile hata detaylarını içerir.
 *
 * Özellikler:
 * - Status: Yanıt durumunu belirtir (ör. "Error", "ValidationError").
 * - Message: Yanıtla ilgili kısa bir açıklama sağlar.
 * - Errors (ValidationApiResponse): Doğrulama hataları listesini içerir.
 */
public abstract class ApiResponse
{
    public string Status { get; set; }
    public string Message { get; set; }
}

public class ErrorApiResponse : ApiResponse
{
    public ErrorApiResponse(string message)
    {
        Status = "Error";
        Message = message;
    }
}

public class ValidationApiResponse : ErrorApiResponse
{
    public List<string> Errors { get; }

    public ValidationApiResponse(List<string> errors) : base("Validation Error")
    {
        Status = "ValidationError";
        Errors = errors;
    }
}