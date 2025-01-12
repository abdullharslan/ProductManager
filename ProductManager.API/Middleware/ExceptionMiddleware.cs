// ProductManager.API/Middleware/ExceptionMiddleware.cs

using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using ProductManager.API.Models;
using ProductManager.Application.Exceptions;

namespace ProductManager.API.Middleware;

/*
 * ExceptionMiddleware, uygulama genelinde gerçekleşen hataları ele alır ve uygun HTTP yanıtları oluşturur.
 * Tüm isteklerin işlenmesi sırasında bir hata oluştuğunda devreye girer ve hatanın türüne göre özel yanıtlar döner.
 *
 * İşlevler:
 * - CustomValidationException: Doğrulama hatalarını 400 Bad Request ile döner.
 * - KeyNotFoundException: Kaynak bulunamadığında 404 Not Found döner.
 * - SecurityTokenException: Geçersiz token durumunda 401 Unauthorized döner.
 * - ArgumentException: Geçersiz argüman durumunda 400 Bad Request döner.
 * - UnauthorizedAccessException: Yetkisiz erişim durumunda 403 Forbidden döner.
 * - Diğer Hatalar: 500 Internal Server Error döner, gelişmiş hata detayları sadece development ortamında gösterilir.
 *
 * Loglama:
 * - Hata türüne bağlı olarak hata detayları ve seviyeleri kaydedilir.
 */
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IWebHostEnvironment _env;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
    
        ErrorApiResponse response;

        switch (exception)
        {
            case CustomValidationException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ValidationApiResponse(ex.ValidationErrors);
                _logger.LogWarning("Validation error: {@ValidationErrors}", ex.ValidationErrors);
                break;

            case KeyNotFoundException ex:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new ErrorApiResponse(ex.Message);
                _logger.LogWarning("Resource not found: {Message}", ex.Message);
                break;

            case SecurityTokenException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = new ErrorApiResponse("Invalid token.");
                _logger.LogWarning("Invalid token attempt");
                break;

            case ArgumentException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ErrorApiResponse(ex.Message);
                _logger.LogWarning("Invalid argument: {Message}", ex.Message);
                break;

            case UnauthorizedAccessException ex:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                response = new ErrorApiResponse("You don't have permission to access this resource.");
                _logger.LogWarning("Unauthorized access attempt: {Message}", ex.Message);
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                // Development ortamında hata detaylarını göster
                if (_env.IsDevelopment())
                {
                    response = new ErrorApiResponse(
                        $"Internal Server Error: {exception.Message}\n" +
                        $"StackTrace: {exception.StackTrace}");
                }
                else
                {
                    response = new ErrorApiResponse("An internal server error occurred.");
                }
                _logger.LogError(exception, "Unhandled exception");
                break;
        }

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsJsonAsync(response, jsonOptions);
    }
}