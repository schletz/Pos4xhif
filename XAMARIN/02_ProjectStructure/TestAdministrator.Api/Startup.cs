using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestAdministrator.Api.Model;
using Microsoft.EntityFrameworkCore;
using TestAdministrator.Api.Extensions;

namespace TestAdministrator.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // SQLite Datenbank mit dem Namen aus appsettings.json konfigurieren. Wird
            // automatisch bei den Controllern im Konstruktor übergeben, wenn sie den Parameter TestsContext
            // erwarten.
            services.AddDbContext<TestsContext>(options =>
                options.UseSqlite($"DataSource={Configuration["AppSettings:Database"]}")
            );

            // Das Secret für den JSON Web Token aus appsettings.json lesen und 
            // den Server dafür konfigurieren.
            services.ConfigureJwt(Configuration["AppSettings:Secret"]);

            // Sollen die Propertynamen nicht in camelCase umgewandelt werden, kann
            //.AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
            // angehängt werden.
            services.AddControllers();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Wird ASPNETCORE_ENVIRONMENT: Development bei environmentVariables gesetzt, wird
            // bei einer Fehlermeldung ein Stacktrace ausgegeben.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // Aktiviert bedeutet: Dateien in wwwroot ausliefern und index.htm als Standarddokument festlegen.
            // app.UseFileServer();

            // Das Routing aktivieren.
            app.UseRouting();

            // Muss NACH UseRouting() und VOR UseEndpoints() stehen.
            app.UseAuthentication();
            app.UseAuthorization();

            // Die in services.AddControllers() geladenen Controller als Endpunkte des Routings registrieren.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
