using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private const string NoValueExceptionMessage = "Don't have exchange rate for requested day";
    private const string DefaultExceptionMessage = "Technical issues on server, please try again later";
    private const string TextPlainContentType = "text/plain";

    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case NotInCircleException e:
                context.Result = new BadRequestObjectResult($"Point [{e.X}, {e.Y}] is not in circle with radius {e.CircleRadius}");
                break;
            case NoValueException:
                context.Result = new BadRequestObjectResult(NoValueExceptionMessage);
                break;
            default:
                context.Result = new ContentResult 
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Content = DefaultExceptionMessage,
                    ContentType = TextPlainContentType
                };
                break;
        }
    }
}