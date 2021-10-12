using Bogus;
using Bogus.Extensions;
using ExamManager.App.Extensions;
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
            optionsBuilder.UseLazyLoadingProxies();
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
                .Rules((f, sc) =>
            {
                var room = $"{f.Random.String2(1, "ABCD")}{f.Random.String2(1, "E1234")}.{f.Random.String2(1, "01")}{f.Random.Int(1, 9)}";
                sc.Room = room.OrDefault(f, 0.2f);

                int i = 100;
                var students = new Faker<Student>("de")
                    .CustomInstantiator(f2 =>
                    {
                        var lastname = f2.Name.LastName();
                        var firstname = f2.Name.FirstName();
                        var account = $"{(lastname.Length < 3 ? lastname : lastname.Substring(0, 3)).ToUpper()}{i:0000}";
                        var dateOfBirth = new DateTime(2000, 1, 1)
                            .AddDays(f2.Random.Int(0, 5 * 365));
                        i++;
                        var home = new Address(
                            f2.Address.City(),
                            f2.Random.Int(1000, 9999).ToString(),
                            f2.Address.StreetAddress());

                        return new Student(
                            account: account,
                            lastname: lastname,
                            firstname: firstname,
                            home: home,
                            dateOfBirth: dateOfBirth);
                    })
                    .Rules((f2, s) =>
                    {
                        s.Email = f2.Internet.Email();
                        var parents = new Address(
                            f2.Address.City(),
                            f2.Random.Int(1000, 9999).ToString(),
                            f2.Address.StreetAddress());
                        s.Parents = parents.OrDefault(f2, 0.25f);
                    })
                    .Generate(20)
                    .ToList();
                sc.Students.AddRange(students);
            })
            .Generate(20)
            .GroupBy(s => s.Name)
            .Select(g => g.First())
            .Take(10)
            .ToList();
            SchoolClasses.AddRange(classes);
            SaveChanges();
        }

    }
}
