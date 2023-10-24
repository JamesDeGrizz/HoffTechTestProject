namespace Application.Exceptions;

public class NotInitializedException : ApplicationException
{
    public NotInitializedException()
            : base()
    { }

    public NotInitializedException(string? message)
        : base(message)
    { }

    public NotInitializedException(string? message, Exception? innerException)
        : base(message, innerException)
    { }
}
