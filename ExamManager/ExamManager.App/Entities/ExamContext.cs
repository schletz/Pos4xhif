using Bogus;
using Bogus.Extensions;
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
        public DbSet<SchoolClass> SchoolClasses => Set<SchoolClass>();
        public DbSet<Exam> Exam => Set<Exam>();
        // SELECT * FROM Exam WHERE Discriminator = 'CommitedExam'
        public DbSet<CommitedExam> CommitedExams => Set<CommitedExam>();

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

        public void Seed()
        {
            Randomizer.Seed = new Random(1035);
            var departments = new string[] { "HIF", "HBGM", "HMNA" };
            var classes = new Faker<SchoolClass>("de")
                .CustomInstantiator(f =>
                {
                    var name = $"{f.Random.Int(1, 5)}{f.Random.String2(1, "ABCDE")}{f.Random.ListItem(departments)}";
                    return new SchoolClass(name);
                })
                .Rules((f, s) =>
            {
                var room = $"{f.Random.String2(1, "ABCD")}{f.Random.String2(1, "E1234")}.{f.Random.String2(1, "01")}{f.Random.Int(1, 9)}";
                s.Room = room.OrDefault(f, 0.2f);
            })
            .Generate(10)
            .GroupBy(s => s.Name)
            .Select(g => g.First())
            .ToList();
            SchoolClasses.AddRange(classes);
            SaveChanges();
        }

    }
}
