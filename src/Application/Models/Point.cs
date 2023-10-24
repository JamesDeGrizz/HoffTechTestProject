namespace Application.Models;

public class Point
{
    public Point(double x, double y)
        => (X, Y) = (x, y);

    public double X { get; set; }
    public double Y { get; set; }
}
