using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AuthExample.App.Model
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

        public virtual DbSet<Klasse> Klasse { get; set; }
        public virtual DbSet<Schueler> Schueler { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Klasse>(entity =>
            {
                entity.HasKey(e => e.K_Name);

                entity.Property(e => e.K_Name).HasColumnType("VARCHAR(10)");

                entity.Property(e => e.K_Abteilung)
                    .IsRequired()
                    .HasColumnType("VARCHAR(200)");
            });

            modelBuilder.Entity<Schueler>(entity =>
            {
                entity.HasKey(e => e.S_Nr);

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

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.U_ID);

                entity.HasIndex(e => e.U_Name)
                    .HasName("idx_u_name")
                    .IsUnique();

                // Änderung auf ValueGeneratedOnAdd() da AUTOINCREMENT von Scaffold nicht gelesen wird.
                entity.Property(e => e.U_ID)
                    .IsRequired()
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.U_Hash)
                    .IsRequired()
                    .HasColumnType("CHAR(64)");

                entity.Property(e => e.U_Name)
                    .IsRequired()
                    .HasColumnType("VARCHAR(100)");

                entity.Property(e => e.U_Role).HasColumnType("VARCHAR(50)");

                entity.Property(e => e.U_Salt)
                    .IsRequired()
                    .HasColumnType("CHAR(32)");

                entity.HasOne(d => d.U_Schueler_NrNavigation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.U_Schueler_Nr);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
