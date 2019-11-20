using Microsoft.EntityFrameworkCore;
using SPG.CodeFirstApplication.Configurations;
using SPG.CodeFirstApplication.Entities;

namespace SPG.CodeFirstApplication
{
    public class SchoolContext : DbContext
    {
        public DbSet<Pupil> Pupils { get; set; }

        public SchoolContext(DbContextOptions<SchoolContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PupilConfiguration());
        }
    }
}
