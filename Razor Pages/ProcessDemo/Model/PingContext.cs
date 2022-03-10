using Microsoft.EntityFrameworkCore;

namespace ProcessDemo.Model
{
    public class PingContext : DbContext
    {
        public DbSet<Job> Jobs => Set<Job>();
        public DbSet<PingResult> PingResults => Set<PingResult>();
        public PingContext(DbContextOptions opt) : base(opt) { }

    }
}
