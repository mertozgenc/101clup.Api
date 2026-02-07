using Microsoft.EntityFrameworkCore;
using _101clup.Api.Models;

namespace _101clup.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    }
}
