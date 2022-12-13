using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
        public DbSet<Questionnaire> Questionnaires => Set<Questionnaire>();
        protected DbSet<QuestionnaireState> QuestionnaireStates => Set<QuestionnaireState>();
        protected DbSet<ActiveQuestionnaire> ActiveQuestionnaires => Set<ActiveQuestionnaire>();
        protected DbSet<ClosedQuestionnaire> ClosedQuestionnaires => Set<ClosedQuestionnaire>();

        public SurveyContext(DbContextOptions<SurveyContext> opt) : base(opt)
        { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Unique constraint. Useful for lookup tables to prevent duplicates.
            modelBuilder.Entity<Schoolclass>().HasIndex(s => s.Name).IsUnique();
            // Configures a value object. Write OwnsMany() for a list of value objects (e. g. addresses)
            modelBuilder.Entity<Student>().OwnsOne(s => s.Name);
            // To write the string repesentation of the enum instead of writing the int value.
            // If you write the number, and you change your enum, the meaning of the data will be changed!
            modelBuilder.Entity<Student>().Property(s => s.Role).HasConversion<string>();

            // Generic config for all entities
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // ON DELETE RESTRICT instead of ON DELETE CASCADE
                foreach (var key in entityType.GetForeignKeys())
                    key.DeleteBehavior = DeleteBehavior.Restrict;

                foreach (var prop in entityType.GetDeclaredProperties())
                {
                    // Define Guid as alternate key. The database can create a guid fou you.
                    if (prop.Name == "Guid")
                    {
                        modelBuilder.Entity(entityType.ClrType).HasAlternateKey("Guid");
                        prop.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd;
                    }
                    // Default MaxLength of string Properties is 255.
                    if (prop.ClrType == typeof(string) && prop.GetMaxLength() is null) prop.SetMaxLength(255);
                    // Seconds with 3 fractional digits.
                    if (prop.ClrType == typeof(DateTime)) prop.SetPrecision(3);
                    if (prop.ClrType == typeof(DateTime?)) prop.SetPrecision(3);
                }
            }
        }
    }
}
