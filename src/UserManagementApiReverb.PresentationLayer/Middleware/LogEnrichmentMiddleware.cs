using Microsoft.AspNetCore.Mvc.Controllers;
using Serilog;
using Serilog.Context;
using UserManagementApiReverb.BusinessLayer.Logging;

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

        var category = controller switch
        {
            "Auth"
                => LogCategories.Security,
            "User" or "Role" or "UserRole" or "Users" or "Roles"
                => LogCategories.Audit,
            "Business" // deneme
                => LogCategories.Business,
            "Performance" // deneme
                => LogCategories.Performance,
            
            _ => LogCategories.Application
                
        };

        using (LogContext.PushProperty("Operation", operation))
        using (LogContext.PushProperty("EventType", eventType))
        using (LogContext.PushProperty("Category", category))
        {
            await _next(context);
        }
    }
    
}