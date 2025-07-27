using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Data;
using UserManagementService.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add gRPC support
builder.Services.AddGrpc();

// Required for HTTP/2 support with gRPC
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5003, o => o.Protocols = HttpProtocols.Http1AndHttp2);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<UserPermissionGrpcService>();

app.Run();