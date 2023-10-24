using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Application.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder AddRequestsLogging(this IApplicationBuilder appBuilder)
    {
        appBuilder.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                var queryParameters = httpContext.Request.Query.Select(x => x.ToString());
                var queryParametersStr = string.Join(",", queryParameters.ToArray());
                diagnosticContext.Set("Query", queryParametersStr);
            };
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} {Query} responded {StatusCode} in {Elapsed:0.0000} ms";
        });

        return appBuilder;
    }
}
