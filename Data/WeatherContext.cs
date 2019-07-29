using Microsoft.EntityFrameworkCore;

namespace TorontoBeachPredictor.Data
{
    public class WeatherContext : DbContext
    {
        public DbSet<WeatherStation> WeatherStations { get; set; }
        public DbSet<WeatherSample> WeatherSamples { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite("Data Source=data.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<WeatherStation>().HasIndex(x => x.Name);
            var weatherSampleEntityTypeBuilder = modelBuilder.Entity<WeatherSample>();
            weatherSampleEntityTypeBuilder.HasIndex(x => x.Date);
            weatherSampleEntityTypeBuilder.HasIndex(x => x.WeatherStationId);
        }
    }
}
