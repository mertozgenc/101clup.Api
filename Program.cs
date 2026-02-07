using Microsoft.EntityFrameworkCore;
using _101clup.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// =====================
// PostgreSQL (Render FIX)
// =====================
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrWhiteSpace(databaseUrl))
    throw new Exception("DATABASE_URL not found");

// 🔥 URI → NPGSQL FORMAT
var uri = new Uri(databaseUrl);
var userInfo = uri.UserInfo.Split(':');

var connectionString =
    $"Host={uri.Host};" +
    $"Port={uri.Port};" +
    $"Database={uri.AbsolutePath.TrimStart('/')};" +
    $"Username={userInfo[0]};" +
    $"Password={userInfo[1]};" +
    $"SSL Mode=Require;Trust Server Certificate=true";

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

// Admin Key
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/api/menu") &&
        ctx.Request.Method != HttpMethods.Get)
    {
        var expected = Environment.GetEnvironmentVariable("ADMIN_KEY") ?? "101clup";
        var provided = ctx.Request.Headers["X-Admin-Key"].ToString();

        if (provided != expected)
        {
            ctx.Response.StatusCode = 401;
            await ctx.Response.WriteAsync("Unauthorized");
            return;
        }
    }

    await next();
});

app.MapControllers();

// =====================
// MIGRATION (SAFE)
// =====================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
