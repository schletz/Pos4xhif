using ExamManager.App.Entities;
using ExamManager.App.Mappings;
using ExamManager.App.Repositories;
using ExamManager.Webapp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StoreManager.Application.Infrastructure;

public static class Program
{
    public static void Main(string[] args)
    {
        using (var context = new ExamContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Seed(new CryptService());
        }

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddTransient<ICryptService, CryptService>();
        builder.Services.AddTransient(provider => new AuthService(
            isDevelopment: builder.Environment.IsDevelopment(),
            db: provider.GetRequiredService<ExamContext>(),
            crypt: provider.GetRequiredService<ICryptService>(),
            httpContextAccessor: provider.GetRequiredService<IHttpContextAccessor>()));
        builder.Services.AddTransient<ExamRepository>();
        builder.Services.AddDbContext<ExamContext>();
        builder.Services.AddAutoMapper(typeof(DtoMappings));

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddAuthentication(
            Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(o =>
            {
                o.LoginPath = "/User/Login";
                o.AccessDeniedPath = "/User/AccessDenied";
            });
        builder.Services.AddAuthorization(o =>
        {
            //o.AddPolicy("OwnerOrAdminRole", p => p.RequireRole(Usertype.Owner.ToString(), Usertype.Admin.ToString()));
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        // Middleware
        app.UseHttpsRedirection();
        app.UseStaticFiles();  // no default document

        app.UseRouting();
        // WepAPI and MVC
        app.MapControllers();
        // Razor Page
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();
        app.Run();
    }
}

