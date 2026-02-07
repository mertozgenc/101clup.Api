using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace _101clup.Api.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // SADECE migration üretmek için DUMMY postgres
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=dummy;Username=dummy;Password=dummy"
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
