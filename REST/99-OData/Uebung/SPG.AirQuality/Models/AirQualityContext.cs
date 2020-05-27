using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SPG.AirQuality.Models
{
    public partial class AirQualityContext : DbContext
    {
        public AirQualityContext()
        { }

        public AirQualityContext(DbContextOptions<AirQualityContext> options)
            : base(options)
        { }

        public virtual DbSet<Measurements> Measurements { get; set; }
        public virtual DbSet<Stations> Stations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Measurements>(entity =>
            {
                entity.HasIndex(x => x.StationId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
