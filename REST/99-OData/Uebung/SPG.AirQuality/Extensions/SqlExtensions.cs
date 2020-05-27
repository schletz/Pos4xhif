using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SPG.AirQuality.Models;

namespace SPG.AirQuality.Extensions
{
    public static class SqlExtensions
    {
        public static void ConfigureMsSql(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AirQualityContext>(optionsBuilder =>
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder
                        .UseSqlite(connectionString);
                }
            });
        }
    }
}
