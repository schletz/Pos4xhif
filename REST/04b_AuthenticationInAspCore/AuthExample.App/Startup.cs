using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using AuthExample.App.Extensions;

namespace AuthExample.App
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureDatabase(Configuration["AppSettings:Database"]);
            services.ConfigureJwt(Configuration["AppSettings:Secret"]);
            services.ConfigureCors();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Dateien in wwwroot ausliefern und index.htm als Standarddokument festlegen.
            app.UseFileServer();
            // Das Routing aktivieren.
            app.UseRouting();

            // Muss NACH UseRouting() und VOR UseEndpoints() stehen.
            app.UseAuthentication();
            app.UseAuthorization();

            // Eigene Regel "CorsPolicy", die in services.ConfigureCors() geladen wurde.
            app.UseCors("CorsPolicy");

            // Die in services.AddControllers() geladenen Controller als Endpunkte des Routings registrieren.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
