using Microsoft.EntityFrameworkCore;

namespace WeatherMap.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        //public DbSet<SuaClasse> nome { get; set; }

        // Define your DbSets here. For example:
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
}