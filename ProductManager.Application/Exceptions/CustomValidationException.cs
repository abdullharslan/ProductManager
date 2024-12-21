// ProductManager.Application/Exceptions/CustomValidationException.cs
namespace ProductManager.Application.Exceptions;

/*
 * CustomValidationException, validasyon hatalarını standart bir formatta tutmak için
 * kullanılan özel exception sınıfıdır.
 */
public class CustomValidationException : Exception
{
    public List<string> ValidationErrors { get; }

    public CustomValidationException(List<string> errors) : base("One or more validation errors occurred.")
    {
        ValidationErrors = errors;
    }

    public CustomValidationException(string error) : base(error)
    {
        ValidationErrors = new List<string> { error };
    }
}