namespace Application.Exceptions;

public class NoValueException : ApplicationException
{
    public NoValueException()
            : base()
    { }

    public NoValueException(string? message)
        : base(message)
    { }

    public NoValueException(string? message, Exception? innerException)
        : base(message, innerException)
    { }
}
