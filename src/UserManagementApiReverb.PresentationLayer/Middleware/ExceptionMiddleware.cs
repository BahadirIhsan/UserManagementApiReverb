using System.Net;
using System.Text.Json;
using Amazon.Runtime;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using UserManagementApiReverb.BusinessLayer.CloudWatchMetricsService;
using UserManagementApiReverb.BusinessLayer.DTOs;

namespace UserManagementApiReverb.PresentationLayer.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;
    private readonly ICloudWatchMetricsService _metricsService;


    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env,  ICloudWatchMetricsService metricsService)
    {
        _next = next;
        _logger = logger;
        _env = env;
        _metricsService = metricsService;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "Beklenmeyen bir hata meydana geldi. Lütfen daha sonra tekrar deneyin.";
            var logMessage = "Genel Hata - Exception middleware yakaladı.";
            object? details = null;
            
            var endpoint = context.Request.Path.Value;
            
            switch (ex)
            {
                case ArgumentNullException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    message = "Eksik veya geçersiz parametre gönderildi.";
                    logMessage = "400 Bad Request - Parametre hatası";
                    break;
                
                case UnauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    message = "Yetkisiz erişim. Lütfen tekrar giriş yapın.";
                    logMessage = "401 UNAUTHORIZED - Token/yetki hatası.";
                    break;
                
                case KeyNotFoundException:
                    statusCode = (int)HttpStatusCode.NotFound;
                    message = "İstenen veri bulunamadı.";
                    logMessage = "404 NOT FOUND - Veri yok.";
                    break;
                
                case DbUpdateException:
                    statusCode = 500;
                    message = "Veritabanı hatası oluştu.";
                    break;
                
                case AmazonServiceException:
                    statusCode = 503;
                    message = "Harici servis geçici olarak kullanılamıyor (AWS).";
                    break;
                
                case SecurityTokenExpiredException:
                    statusCode = 401;
                    message = "Oturum süresi dolmuş. Lütfen tekrar giriş yapın.";
                    break;
                
                case FluentValidation.ValidationException validationEx:
                    statusCode = 400;
                    message = "Geçersiz istek verisi.";
                    details = validationEx.Errors.Select(e => new
                    {
                        property = e.PropertyName,
                        error = e.ErrorMessage
                    });
                    break;
                
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    message = "Beklenmeyen bir sunucu hatası oluştu.";
                    logMessage = "500 INTERNAL SERVER ERROR - Bilinmeyen hata.";
                    break;
                
            }

            if (_env.IsDevelopment())
            {
                _logger.LogError(ex, "DEVELOPMENT: {LogMessage} - {Error}", logMessage, ex.Message);
            }
            else if (_env.IsProduction())
            {
                _logger.LogError("PRODUCTION: {LogMessage}", logMessage);
            }

            object errorResponse = _env.IsDevelopment()
                ? new
                {
                    StatusCode = statusCode,
                    Message = message,
                    ExceptionMessage = ex.Message,
                    ExceptionType = ex.GetType().Name,
                    StackTrace = ex.StackTrace,
                    Details = details
                }
                : new
                {
                    StatusCode = statusCode,
                    Message = message
                };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
            
            var json = JsonSerializer.Serialize(errorResponse, jsonOptions);
            context.Response.StatusCode = statusCode;
            
            try
            {
                await _metricsService.SendErrorMetricAsync(endpoint ?? "Unknown", statusCode);
            }
            catch (Exception metricEx)
            {
                _logger.LogError(metricEx, "Metric gönderimi sırasında hata oluştu.");
            }
            
            await context.Response.WriteAsync(json);
        }
        
    }
    
    
}