using Microsoft.EntityFrameworkCore;

namespace TorontoBeachPredictor.Data
{
    public class BeachContext : DbContext
    {
        public DbSet<Beach> Beaches { get; set; }
        public DbSet<BeachSample> BeachSamples { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlite("Data Source=data.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var entityTypeBuilder = modelBuilder.Entity<BeachSample>();
            entityTypeBuilder.HasIndex(x => x.SampleDate);
            entityTypeBuilder.HasIndex(x => x.PublishDate);
            entityTypeBuilder.HasIndex(x => x.EColiCount);
            entityTypeBuilder.HasIndex(x => x.BeachStatus);
            entityTypeBuilder.HasIndex(x => x.BeachId);
        }
    }
}
