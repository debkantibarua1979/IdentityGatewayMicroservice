using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserManagementService.Data;
using UserManagementService.Dtos;
using UserManagementService.GrpcServers;
using UserManagementService.Repositories.Implementations;
using UserManagementService.Repositories.Interfaces;
using UserManagementService.Services.Implementations;
using UserManagementService.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ===== Configuration =====
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.AddHttpContextAccessor();

// ===== Kestrel Setup for HTTP/2 (gRPC) =====
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5005, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

// ===== Database =====
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== Repositories =====
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();


// ===== Services =====
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();

// ===== gRPC Server =====
builder.Services.AddGrpc();

// ===== Swagger (Optional for Testing REST Endpoints) =====
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UserManagementService", Version = "v1" });
});

var app = builder.Build();


// ===== Middleware =====
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// ===== gRPC Map =====
app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<UserServiceGrpc>();
});

app.Run();
