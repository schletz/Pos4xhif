using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Plf4bhif20191125.Testapp.Model
{
    public partial class RegistrationContext : DbContext
    {
        public RegistrationContext()
        {
        }

        public RegistrationContext(DbContextOptions<RegistrationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Registration> Registration { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("DataSource=../Registration.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Registration>(entity =>
            {
                entity.HasIndex(e => e.R_Department)
                    .HasName("DepartmentRegistration");

                entity.Property(e => e.R_ID).ValueGeneratedOnAdd();

                entity.HasOne(d => d.R_DepartmentNavigation)
                    .WithMany(p => p.Registration)
                    .HasForeignKey(d => d.R_Department)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
