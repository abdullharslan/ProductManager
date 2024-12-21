// ProductManager.API/Models/ApiResponse.cs

namespace ProductManager.API.Models;

/*
 * API yanıtları için standart response modelleri.
 * Tüm API yanıtları bu formatta dönecek.
 */
public class ApiResponse
{
    public string Status { get; set; }
    public string Message { get; set; }
}

public class ValidationApiResponse : ApiResponse
{
    public List<string> Errors { get; set; }

    public ValidationApiResponse(List<string> errors)
    {
        Status = "ValidationError";
        Errors = errors;
    }
}

public class ErrorApiResponse : ApiResponse
{
    public ErrorApiResponse(string message)
    {
        Status = "Error";
        Message = message;
    }
}