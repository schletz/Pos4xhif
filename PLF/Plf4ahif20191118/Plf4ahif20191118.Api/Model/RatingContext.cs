using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Plf4ahif20191118.Model
{
    public partial class RatingContext : DbContext
    {
        public RatingContext()
        {
        }

        public RatingContext(DbContextOptions<RatingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<School> School { get; set; }
        public virtual DbSet<School_Rating> School_Rating { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("DataSource=../Rating.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<School>(entity =>
            {
                entity.Property(e => e.S_Nr).ValueGeneratedNever();
            });

            modelBuilder.Entity<School_Rating>(entity =>
            {
                entity.HasIndex(e => new { e.SR_User_Phonenr, e.SR_User_School })
                    .HasName("UserSchool_Rating");

                entity.Property(e => e.SR_ID).ValueGeneratedOnAdd();

                entity.HasOne(d => d.SR_User_Navigation)
                    .WithMany(p => p.School_Rating)
                    .HasForeignKey(d => new { d.SR_User_Phonenr, d.SR_User_School })
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => new { e.U_Phonenr, e.U_School });

                entity.HasIndex(e => e.U_School)
                    .HasName("SchoolUser");

                entity.HasOne(d => d.U_SchoolNavigation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.U_School)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
