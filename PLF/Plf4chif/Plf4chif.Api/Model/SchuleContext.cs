using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Plf4chif.Api.Model
{
    public partial class SchuleContext : DbContext
    {
        public SchuleContext()
        {
        }

        public SchuleContext(DbContextOptions<SchuleContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Fach> Faecher { get; set; }
        public virtual DbSet<Klasse> Klassen { get; set; }
        public virtual DbSet<Pruefung> Pruefungen { get; set; }
        public virtual DbSet<Schueler> Schuelers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("DataSource=../Schule.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fach>(entity =>
            {
                entity.HasKey(e => e.F_ID);

                entity.HasIndex(e => e.F_Name)
                    .HasName("idx_F_Name")
                    .IsUnique();

                entity.Property(e => e.F_ID).HasColumnType("VARCHAR(10)");

                entity.Property(e => e.F_Name).HasColumnType("VARCHAR(200)");
            });

            modelBuilder.Entity<Klasse>(entity =>
            {
                entity.HasKey(e => e.K_Name);

                entity.Property(e => e.K_Name).HasColumnType("VARCHAR(10)");

                entity.Property(e => e.K_Abteilung)
                    .IsRequired()
                    .HasColumnType("VARCHAR(200)");
            });

            modelBuilder.Entity<Pruefung>(entity =>
            {
                entity.HasKey(e => e.P_ID);

                entity.HasIndex(e => e.P_Fach)
                    .HasName("FaecherPruefungen");

                entity.HasIndex(e => e.P_Schueler)
                    .HasName("SchuelerPruefungen");

                entity.Property(e => e.P_ID).ValueGeneratedOnAdd();

                entity.Property(e => e.P_Datum)
                    .IsRequired()
                    .HasColumnType("TIMESTAMP");

                entity.Property(e => e.P_Fach)
                    .IsRequired()
                    .HasColumnType("VARCHAR(10)");

                entity.Property(e => e.P_Note).HasDefaultValueSql("0");

                entity.HasOne(d => d.P_FachNavigation)
                    .WithMany(p => p.Pruefung)
                    .HasForeignKey(d => d.P_Fach)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.P_SchuelerNavigation)
                    .WithMany(p => p.Pruefung)
                    .HasForeignKey(d => d.P_Schueler)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Schueler>(entity =>
            {
                entity.HasKey(e => e.S_Nr);

                entity.HasIndex(e => e.S_Klasse)
                    .HasName("fk_Klasse_Klasse_0");

                entity.Property(e => e.S_Nr).ValueGeneratedNever();

                entity.Property(e => e.S_Geschl)
                    .IsRequired()
                    .HasColumnType("CHAR(1)");

                entity.Property(e => e.S_Klasse)
                    .IsRequired()
                    .HasColumnType("VARCHAR(10)");

                entity.Property(e => e.S_Vorname)
                    .IsRequired()
                    .HasColumnType("VARCHAR(200)");

                entity.Property(e => e.S_Zuname)
                    .IsRequired()
                    .HasColumnType("VARCHAR(200)");

                entity.HasOne(d => d.S_KlasseNavigation)
                    .WithMany(p => p.Schueler)
                    .HasForeignKey(d => d.S_Klasse)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
