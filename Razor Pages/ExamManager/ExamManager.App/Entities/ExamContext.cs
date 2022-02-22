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
    /// <summary>
    /// EF Core Database Context
    /// </summary>
    public class ExamContext : DbContext
    {
        public DbSet<SchoolClass> SchoolClasses => Set<SchoolClass>();
        public DbSet<Exam> Exam => Set<Exam>();
        // SELECT * FROM Exam WHERE Discriminator = 'CommitedExam'
        public DbSet<CommitedExam> CommitedExams => Set<CommitedExam>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Subject> Subjects => Set<Subject>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Needs NuGet package Microsoft.EntityFrameworkCore.Proxies
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
            modelBuilder.Entity<Student>().HasIndex(s => s.Guid).IsUnique();
            modelBuilder.Entity<Teacher>().HasIndex(t => t.Shortname).IsUnique();
            modelBuilder.Entity<SchoolClass>().HasIndex(s => s.Name).IsUnique();
            modelBuilder.Entity<Subject>().HasIndex(s => s.Name).IsUnique();

            // Exclude inherited properties
            var searchFlag = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly;
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var type = entity.ClrType;
                if (type.GetProperty("Guid", searchFlag) is not null)
                {
                    modelBuilder.Entity(type).HasAlternateKey("Guid");
                    modelBuilder.Entity(type).Property("Guid").ValueGeneratedOnAdd();
                }
            }

        }

        public void Seed()
        {
            Randomizer.Seed = new Random(1035);
            var faker = new Faker("de");

            var teachers = new Faker<Teacher>("de")
                .CustomInstantiator(f =>
                {
                    var lastname = f.Name.LastName();
                    var shortname = lastname.Left(3).ToUpper();

                    return new Teacher(
                        shortname: shortname,
                        firstname: f.Name.FirstName(),
                        lastname: lastname,
                        email: f.Internet.Email());
                })
                .Generate(50)
                .GroupBy(t => t.Shortname)
                .Select(g => g.First())
                .Take(30)
                .ToList();
            Teachers.AddRange(teachers);
            SaveChanges();

            var subjects = new Faker<Subject>("de")
                .CustomInstantiator(f =>
                {
                    var name = f.Commerce.ProductMaterial();
                    var shortname = name.Left(3).ToUpper();

                    return new Subject(
                        shortname: shortname,
                        name: name);
                })
                .Generate(50)
                .GroupBy(t => t.Shortname)
                .Select(g => g.First())
                .Take(10)
                .ToList();
            Subjects.AddRange(subjects);
            SaveChanges();

            var departments = new string[] { "HIF", "HBGM", "HMNA" };
            var classes = new Faker<SchoolClass>("de")
                .CustomInstantiator(f =>
                {
                    var name = $"{f.Random.Int(1, 5)}{f.Random.String2(1, "ABCDE")}{f.Random.ListItem(departments)}";
                    return new SchoolClass(name, f.Random.ListItem(teachers));
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
                            dateOfBirth: dateOfBirth,
                            schoolClass: sc);
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

            // 20 exams per class -> 200 exams
            var exams = new Faker<Exam>("de")
                .CustomInstantiator(f =>
                {
                    return new Exam(
                        teacher: f.Random.ListItem(teachers),
                        subject: f.Random.ListItem(subjects),
                        date: new DateTime(2021, 10, 1)
                            .AddDays(f.Random.Int(0, 30 * 9))
                            .AddHours(f.Random.Int(8, 16)),
                        schoolClass: f.Random.ListItem(classes));
                })
                .Generate(200)
                .ToList();
            Exam.AddRange(exams.Take(100));
            SaveChanges();

            CommitedExams.AddRange(exams
                .Skip(100)
                .Select(e =>
                {
                    var room = $"{faker.Random.String2(1, "ABCD")}{faker.Random.String2(1, "E1234")}.{faker.Random.String2(1, "01")}{faker.Random.Int(1, 9)}";
                    return new CommitedExam(e, room);
                }));
            SaveChanges();
        }

    }
}
