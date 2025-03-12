namespace WebFormManager.Infrastructure.Exceptions;

public class FileStorageException : Exception
{
    public FileStorageException(string message, Exception innerException)
        : base(message, innerException) { }
}