using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using SchoolManager.Extensions;
using SchoolManager.Model;
using System.Linq;

namespace SchoolManager
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
            services.AddControllers(options => options.EnableEndpointRouting = false);

            services.ConfigureMsSql(Configuration["sqlConnection:connectionString"]);

            services.AddOData();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});

            app.UseMvc(routeBuilder => 
            {
                routeBuilder.MapODataServiceRoute("odata", "odata", GetEdmModel());
            });
        }

        private IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            EntitySetConfiguration<SchoolClasses> schoolClasses = builder.EntitySet<SchoolClasses>("SchoolClasses");
            schoolClasses.EntityType.Count().Filter().OrderBy().Expand().Select();

            EntitySetConfiguration<Pupils> pupils = builder.EntitySet<Pupils>("Pupils");
            pupils.EntityType.Count().Filter().OrderBy().Select();

            return builder.GetEdmModel();
        }
    }
}
