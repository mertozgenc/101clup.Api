using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DATABASE
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrWhiteSpace(databaseUrl))
    throw new Exception("DATABASE_URL not set");

// Render postgres:// → Npgsql formatına çevir
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
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

// Admin Key middleware
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/api/menu") &&
        HttpMethods.IsPost(ctx.Request.Method))
    {
        var adminKey = Environment.GetEnvironmentVariable("ADMIN_KEY") ?? "";
        var provided = ctx.Request.Headers["X-Admin-Key"].ToString();

        if (provided != adminKey)
        {
            ctx.Response.StatusCode = 401;
            await ctx.Response.WriteAsync("Unauthorized");
            return;
        }
    }

    await next();
});

app.MapControllers();

// 🔥 BURASI ÖNEMLİ: SADECE MIGRATE
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
