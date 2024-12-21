// ProductManager.API/Middleware/ExceptionMiddleware.cs

using ProductManager.API.Models;
using ProductManager.Application.Exceptions;

namespace ProductManager.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        object response;

        switch (exception)
        {
            case CustomValidationException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ValidationApiResponse(ex.ValidationErrors);
                break;

            case KeyNotFoundException ex:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new ErrorApiResponse(ex.Message);
                break;

            case ArgumentException ex:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ErrorApiResponse(ex.Message);
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new ErrorApiResponse("An internal server error occurred.");
                break;
        }

        await context.Response.WriteAsJsonAsync(response);
    }
}