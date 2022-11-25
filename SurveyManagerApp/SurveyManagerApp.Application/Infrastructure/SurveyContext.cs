using Microsoft.EntityFrameworkCore;
using SurveyManagerApp.Application.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurveyManagerApp.Application.Infrastructure
{
    public class SurveyContext : DbContext
    {
        public static SurveyContext ForTestConfig()
        {
            // Configure for our sql server container
            // docker run -d -p 1433:1433  --name sqlserver2019 -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=SqlServer2019" mcr.microsoft.com/mssql/server:2019-latest    
            var opt = new DbContextOptionsBuilder<SurveyContext>()
                .UseSqlServer(@"Server=127.0.0.1,1433;Initial Catalog=SurveyTestDb;User Id=sa;Password=SqlServer2019")
                .Options;
            return new SurveyContext(opt);
        }
        // DO NOT register DbSet<Name> because its a value object!
        public DbSet<Schoolclass> Schoolclasses => Set<Schoolclass>();
        public DbSet<Student> Students => Set<Student>();

        public SurveyContext(DbContextOptions<SurveyContext> opt) : base(opt)
        { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Schoolclass>().HasIndex(s => s.Name).IsUnique();
            // Write OwnsMany() for a list of value objects (e. g. addresses)
            modelBuilder.Entity<Student>().OwnsOne(s => s.Name);
            modelBuilder.Entity<Student>().Property(s => s.Role).HasConversion<string>();
        }
    }
}
