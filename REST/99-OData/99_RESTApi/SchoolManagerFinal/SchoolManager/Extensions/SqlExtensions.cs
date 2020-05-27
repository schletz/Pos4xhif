using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SchoolManager.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManager.Extensions
{
    public static class SqlExtensions
    {
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });

        public static void ConfigureMsSql(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<SchoolContext>(optionsBuilder =>
            {
                if (!optionsBuilder.IsConfigured)
                {
                    optionsBuilder
                        .UseLoggerFactory(MyLoggerFactory)
                        .UseSqlite(connectionString);
                }
            });
        }
    }
}
