using Microsoft.EntityFrameworkCore;
using WeatherMap.Models;

namespace WeatherMap.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<WeatherHistory> WeatherHistories { get; set; }
        public DbSet<DailyForecast> DailyForecasts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da relação 1:N
            modelBuilder.Entity<WeatherHistory>()
                .HasMany(w => w.DailyForecasts)
                .WithOne(d => d.WeatherHistory!)
                .HasForeignKey(d => d.WeatherHistoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurações opcionais (constraints e tamanhos)
            modelBuilder.Entity<WeatherHistory>()
                .Property(w => w.LocationName)
                .HasMaxLength(200);

            modelBuilder.Entity<DailyForecast>()
                .Property(d => d.WeatherDescription)
                .HasMaxLength(500);
        }

        //public DbSet<SuaClasse> nome { get; set; }

        // Define your DbSets here. For example:
        // public DbSet<YourEntity> YourEntities { get; set; }
    }
}