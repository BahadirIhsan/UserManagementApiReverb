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
using UserManagementApiReverb.BusinessLayer.FluentValidation;
using UserManagementApiReverb.BusinessLayer.Logging;
using UserManagementApiReverb.DataAccessLayer.Interceptors;
using UserManagementApiReverb.PresentationLayer.Middleware;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",  new() { Title = "UserManagementApiReverb API", Version = "v1" });
    
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
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserRequestRegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RoleCreateRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RoleUpdateRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserRoleAssignValidator>();





builder.Services.AddControllers();



var region  = builder.Configuration["AWS:Region"]    ?? "eu-north-1";
var logGroup   = builder.Configuration["AWS:LogGroup"]  ?? "UserManagementApiReverb";
var accessKey  = builder.Configuration["AWS:AccessKey"];
var secretKey  = builder.Configuration["AWS:SecretKey"];

var awsCreds   = new BasicAWSCredentials(accessKey, secretKey);

var options = new CloudWatchSinkOptions
{
    LogGroupName = logGroup,
    MinimumLogEventLevel = LogEventLevel.Information,
    TextFormatter = new JsonFormatter(),
    // Her uygulama instance’ı için otomatik stream adı (“hostname-tarih”)
    LogStreamNameProvider = new DefaultLogStreamProvider()
};

var cloudWatchConfig = new AmazonCloudWatchLogsConfig
{
    RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
};

var cloudWatchClient = new AmazonCloudWatchLogsClient(awsCreds, cloudWatchConfig);

// Serilog ve CloudWatch
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()                                // global eşik
    .MinimumLevel.Override("Security", LogEventLevel.Information)
    .MinimumLevel.Override("Audit", LogEventLevel.Information)
    .MinimumLevel.Override("Performance", LogEventLevel.Information)
    .MinimumLevel.Override("Business", LogEventLevel.Information)
    .MinimumLevel.Override("Application", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithClientIp()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Service", "UserManagementApiReverb")
    .WriteTo.AmazonCloudWatch(options, cloudWatchClient) // logları aws cloudwatch'a gönderir
    .WriteTo.Console()
    .CreateLogger();


builder.Host.UseSerilog();

builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);
builder.Services.AddSingleton<IAppLogger, CloudWatchLogger>();
builder.Services.AddSingleton<IAmazonCloudWatchLogs, AmazonCloudWatchLogsClient>();
builder.Services.AddScoped<ILogQueryService, LogQueryService>();


var app = builder.Build();

app.UseMiddleware<LogEnrichmentMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
