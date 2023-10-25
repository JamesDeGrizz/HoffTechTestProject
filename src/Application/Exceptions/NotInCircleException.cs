namespace Application.Exceptions;

public class NotInCircleException : ApplicationException
{
    public double X { get; set; }
    public double Y { get; set; }
    public double CircleRadius { get; set; }

    public NotInCircleException()
            : base()
    { }

    public NotInCircleException(double x, double y, double circleRaduis)
            : base()
        => (X, Y, CircleRadius) = (x, y, circleRaduis);

    public NotInCircleException(string? message)
        : base(message)
    { }

    public NotInCircleException(string? message, Exception? innerException)
        : base(message, innerException)
    { }
}
