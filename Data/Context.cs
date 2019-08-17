using Microsoft.EntityFrameworkCore;

namespace TorontoBeachPredictor.Data
{
	public class Context : DbContext
	{
		public DbSet<Beach> Beaches { get; set; }
		public DbSet<BeachSample> BeachSamples { get; set; }
		public DbSet<WeatherStation> WeatherStations { get; set; }
		public DbSet<WeatherSample> WeatherSamples { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
			optionsBuilder.UseSqlite("Data Source=data.db");

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<BeachSample>().HasIndex(x => x.SampleDate);
			modelBuilder.Entity<BeachSample>().HasIndex(x => x.PublishDate);
			modelBuilder.Entity<BeachSample>().HasIndex(x => x.EColiCount);
			modelBuilder.Entity<BeachSample>().HasIndex(x => x.BeachStatus);
			modelBuilder.Entity<BeachSample>().HasIndex(x => x.BeachId);

			modelBuilder.Entity<WeatherStation>().HasIndex(x => x.Name);

			modelBuilder.Entity<WeatherSample>().HasIndex(x => x.Date);
			modelBuilder.Entity<WeatherSample>().HasIndex(x => x.WeatherStationId);
		}
	}
}