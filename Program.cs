using Microsoft.EntityFrameworkCore;
using _101clup.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// =====================
// Services
// =====================

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (Frontend serbest)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// =====================
// PostgreSQL (Render Native)
// =====================
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new Exception("DATABASE_URL not found");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

var app = builder.Build();

// =====================
// Middleware
// =====================

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

// Admin Key Middleware
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/api/menu") &&
        ctx.Request.Method != HttpMethods.Get)
    {
        var expectedKey = Environment.GetEnvironmentVariable("ADMIN_KEY") ?? "101clup";
        var providedKey = ctx.Request.Headers["X-Admin-Key"].ToString();

        if (providedKey != expectedKey)
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await ctx.Response.WriteAsync("Unauthorized");
            return;
        }
    }

    await next();
});

app.MapControllers();

// =====================
// Database Migration
// (Sadece APPLY – ASLA DROP ETMEZ)
// =====================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
