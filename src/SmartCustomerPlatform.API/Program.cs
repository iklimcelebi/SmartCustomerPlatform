using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using SmartCustomerPlatform.API.Middleware;
using SmartCustomerPlatform.Application.DependencyInjection;
using SmartCustomerPlatform.Infrastructure.DependencyInjection;
using SmartCustomerPlatform.Persistence.Contexts;
using SmartCustomerPlatform.Persistence.DependencyInjection;
using SmartCustomerPlatform.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================================================
// APPLICATION / PERSISTENCE / INFRASTRUCTURE
// ============================================================

builder.Services.AddApplicationServices();

builder.Services.AddPersistenceServices(
    builder.Configuration);

builder.Services.AddInfrastructureServices(
    builder.Configuration);

// ============================================================
// JWT AUTHENTICATION
// ============================================================

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key configuration is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "Jwt:Issuer configuration is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "Jwt:Audience configuration is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)),

            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ============================================================
// CONTROLLERS / SWAGGER
// ============================================================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================================================
// BUILD
// ============================================================

var app = builder.Build();

// ============================================================
// HTTP PIPELINE
// ============================================================

app.UseCors("Frontend");

// ============================================================
// DATABASE SEED
// ============================================================

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<SmartCustomerPlatformDbContext>();

    await DepartmentSeed.SeedAsync(context);
}

// ============================================================
// EXCEPTION HANDLING
// ============================================================

app.UseMiddleware<ExceptionHandlingMiddleware>();

// ============================================================
// SWAGGER
// ============================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ============================================================
// HTTPS
// ============================================================

app.UseHttpsRedirection();

// ============================================================
// AUTHENTICATION / AUTHORIZATION
// ============================================================

app.UseAuthentication();
app.UseAuthorization();

// ============================================================
// CONTROLLERS
// ============================================================

app.MapControllers();

app.Run();