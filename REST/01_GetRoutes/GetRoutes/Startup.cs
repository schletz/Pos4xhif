using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetRoutes.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GetRoutes
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Konfiguriert den Server, dass Requests von jeder URL akzeptiert werden
            // (Allow origin: *)
            services.ConfigureCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Aktiviert die in services.ConfigureCors() gesetzte Konfiguration.
            app.UseCors("CorsPolicy");
            app.UseFileServer();
            app.UseMvc();
        }
    }
}
