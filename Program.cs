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
    options.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});

// PostgreSQL (SADECE POSTGRES)
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new Exception("DATABASE_URL not found");

if (connectionString.StartsWith("postgres://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');

    connectionString =
        $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};" +
        $"Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");

// Admin Key middleware
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

// MIGRATION (SADECE CREATE / UPDATE – ASLA SİLMEZ)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
