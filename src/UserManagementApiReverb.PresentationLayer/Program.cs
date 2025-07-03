using Microsoft.EntityFrameworkCore;
using UserManagementApiReverb.BusinessLayer.Mappings;
using UserManagementApiReverb.BusinessLayer.RoleServices;
using UserManagementApiReverb.BusinessLayer.UserRoleService;
using UserManagementApiReverb.BusinessLayer.UserServices;
using UserManagementApiReverb.DataAccessLayer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddScoped<IUserMapper, UserMapper>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRoleMapper, RoleMapper>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IUserRoleMapper, UserRoleMapper>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
