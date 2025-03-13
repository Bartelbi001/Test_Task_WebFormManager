namespace WebFormManager.API.Exceptions;

public class ApiValidationException : Exception
{
    public ApiValidationException(string message) : base(message) { }
}