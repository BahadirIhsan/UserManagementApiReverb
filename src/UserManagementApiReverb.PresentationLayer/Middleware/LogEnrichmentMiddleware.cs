using Microsoft.AspNetCore.Mvc.Controllers;
using Serilog.Context;

namespace UserManagementApiReverb.PresentationLayer.Middleware;

public class LogEnrichmentMiddleware
{
    private readonly RequestDelegate _next;

    public LogEnrichmentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // bu kısım Operation EventType kısımlarını oluşturur.
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var descriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

        var controller = descriptor?.ControllerName ?? "Unknown";
        var action = descriptor?.ActionName ?? "Unknown";

        var operation = $"{controller}.{action}";
        var eventType = action;

        using (LogContext.PushProperty("Operation", operation))
        using (LogContext.PushProperty("EventType", eventType))
        {
            await _next(context);
        }
    }
    
}