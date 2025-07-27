using GatewayService.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using UserManagement.Protos;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var jwtSettings = builder.Configuration.GetSection("JwtOptions");
var secretKey = jwtSettings["SecretKey"];

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication Setup
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey))
        };
    });

// gRPC Client for UserManagementService
builder.Services.AddGrpcClient<UserPermissionService.UserPermissionServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:5005");
});

// Add Ocelot
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<TokenExpirationMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.UseOcelot().Wait();

app.Run();