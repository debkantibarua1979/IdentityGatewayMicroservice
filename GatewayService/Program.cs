using System.Text;
using GatewayService.Clients;
using GatewayService.Middlewares;
using GatewayService.Repositories.Implementations;
using GatewayService.Repositories.Interfaces;
using GatewayService.Services.Implementations;
using GatewayService.Services.Interfaces;
using IdentityService.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Responder;

var builder = WebApplication.CreateBuilder(args);

// ---------- Configuration ----------
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// ---------- JWT Configuration ----------
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>()!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// ---------- Dependency Injection ----------
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<GrpcUserClient>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<TokenExpirationMiddleware>();
// ---------- Ocelot with SafeHttpResponder ----------

builder.Services
    .AddOcelot(builder.Configuration)
    .Services
    .AddSingleton<IHttpResponder, SafeHttpResponder>();

var app = builder.Build();

// ---------- Middleware Pipeline ----------
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<TokenExpirationMiddleware>();
await app.UseOcelot();

app.Run();
