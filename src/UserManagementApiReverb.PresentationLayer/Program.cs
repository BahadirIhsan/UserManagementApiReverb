using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserManagementApiReverb.BusinessLayer.AuthServices;
using UserManagementApiReverb.BusinessLayer.Mappings;
using UserManagementApiReverb.BusinessLayer.RoleServices;
using UserManagementApiReverb.BusinessLayer.UserRoleService;
using UserManagementApiReverb.BusinessLayer.UserServices;
using UserManagementApiReverb.DataAccessLayer;

// JWT İÇİN
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

// AWS CLOUDWATCH İÇİN
using Serilog;
using Serilog.Events;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog.Formatting.Json;
using Serilog.Sinks.AwsCloudWatch;
using UserManagementApiReverb.BusinessLayer.CloudWatchMetricsService;
using UserManagementApiReverb.BusinessLayer.FluentValidation;
using UserManagementApiReverb.BusinessLayer.Logging;
using UserManagementApiReverb.BusinessLayer.Services.Abstract;
using UserManagementApiReverb.BusinessLayer.UserSessionServices;
using UserManagementApiReverb.DataAccessLayer.Interceptors;
using UserManagementApiReverb.PresentationLayer.Middleware;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",  new Microsoft.OpenApi.Models.OpenApiInfo()
    {
        Title = "UserManagementApiReverb API", 
        Version = "v1",
        Description = "Baho'nun Uygulaması",
    });
    
    // xml için tanımlama
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    
    // BL (BusinessLayer) katmanı için de XML bağlayalım:
    var xmlBL = "UserManagementApiReverb.BusinessLayer.xml"; // ← Assembly adına dikkat et!
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlBL));
    
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme.",
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

    });


builder.Services.AddScoped<AuditSaveChangesInterceptor>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<AuditSaveChangesInterceptor>();
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );
    options.AddInterceptors(interceptor); 
});
    

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserMapper, UserMapper>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleMapper, RoleMapper>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IUserRoleMapper, UserRoleMapper>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserSessionService, UserSessionService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserRequestRegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RoleCreateRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RoleUpdateRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserRoleAssignValidator>();


builder.Services.AddControllers();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Console.WriteLine($"Active Environment: {builder.Environment.EnvironmentName}");

var environment = builder.Environment.EnvironmentName;

var region  = builder.Configuration["AWS:Region"] ?? "eu-north-1";
var logGroup = builder.Configuration["AWS:LogGroupName"];// Ortam dosyasından geliyor
var accessKey  = builder.Configuration["AWS:AccessKey"];
var secretKey  = builder.Configuration["AWS:SecretKey"];

var awsCreds   = new BasicAWSCredentials(accessKey, secretKey);

// CloudWatch log ayarları
var cloudWatchOptions = new CloudWatchSinkOptions
{
    LogGroupName = logGroup,
    MinimumLogEventLevel = environment == "Development" ? LogEventLevel.Information : LogEventLevel.Warning,
    TextFormatter = new JsonFormatter(),
    LogStreamNameProvider = new DefaultLogStreamProvider()
};

var cloudWatchConfig = new AmazonCloudWatchLogsConfig
{
    RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
};

var cloudWatchClient = new AmazonCloudWatchLogsClient(awsCreds, cloudWatchConfig);

// Serilog ve CloudWatch
// Serilog ana logger (Category filtreli)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is(environment == "Development" ? LogEventLevel.Debug : LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithClientIp()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Service", "UserManagementApiReverb")
    .WriteTo.Console()
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le => le.Properties.ContainsKey("Category"))
        .WriteTo.AmazonCloudWatch(cloudWatchOptions, cloudWatchClient))
    .CreateLogger();

Console.WriteLine($"LogGroupName: {logGroup}"); // DEBUG: log grubu çıktısı
Console.WriteLine($"Aktif ortam: {environment}"); // DEBUG: console log

Console.WriteLine($"Aktif ortam: {builder.Environment.EnvironmentName}");
Console.WriteLine($"LogGroup: {builder.Configuration["AWS:LogGroupName"]}");




builder.Host.UseSerilog();

builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
builder.Services.AddSingleton<IAppLogger, CloudWatchLogger>();
builder.Services.AddSingleton<IAmazonCloudWatchLogs, AmazonCloudWatchLogsClient>();
builder.Services.AddScoped<ILogQueryService, LogQueryService>();
builder.Services.AddSingleton<ICloudWatchMetricsService, CloudWatchMetricsService>();
builder.Services.AddSingleton<ActiveUserStore>();



var app = builder.Build();

app.UseMiddleware<LogEnrichmentMiddleware>();
app.UseMiddleware<TransactionMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestMetricsMiddleware>();
app.UseMiddleware<ResponseTimeMiddleware>();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "User Management Api Reverb v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ActiveUserTrackingMiddleware>(); // bu middleware'ın diğerlerinden farklı olarak burada olması gerekiyor çünkü bu kısımda yapılan işlem 
// auth işlemleri öncelikle bunların yapılması lazım ki userlar göncellenebilsin ve denetlenebilsin yoksa boşa çalışan bir middleware olur


app.MapControllers();
app.Run();

