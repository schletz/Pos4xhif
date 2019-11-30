using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SPG.CodeFirstApplication
{
    /// <summary>
    /// Eine Factory-Klasse die zur Instanzierung der Klasse <code>SchoolContext</code> herangezogen werden kann.
    /// </summary>
    /// <remarks>
    /// Die Klasse implementiert das Interface <code>IDesignTimeDbContextFactory</code>.
    /// Die zu implementierende Methode instanziert den DbContext mit den entsprechenden Options.
    /// In diesem Fall <Code>UseSqlite</Code>.
    /// </remarks>
    public class SchoolContextFactory : IDesignTimeDbContextFactory<SchoolContext>
    {
        private static string _connectionString;

        public SchoolContext CreateDbContext()
        {
            return CreateDbContext(null);
        }

        public SchoolContext CreateDbContext(string[] args)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                LoadConnectionString();
            }

            var builder = new DbContextOptionsBuilder<SchoolContext>();
            builder.UseSqlite(_connectionString);

            return new SchoolContext(builder.Options);
        }

        private static void LoadConnectionString()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false);

            var configuration = builder.Build();

            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
    }
}
