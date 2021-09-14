using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Entities
{
    public class ExamContext : DbContext
    {
        public DbSet<Student> Students => Set<Student>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Exam.db");
            optionsBuilder.LogTo(
                Console.WriteLine,
                Microsoft.Extensions.Logging.LogLevel.Information);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().OwnsOne(s => s.Home);
            modelBuilder.Entity<Student>().OwnsOne(s => s.Parents);
        }


    }
}
