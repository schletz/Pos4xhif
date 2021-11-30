using ExamManager.App.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public static class Program
{
    public static void Main(string[] args)
    {
        using (var context = new ExamContext())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Seed();
        }

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddDbContext<ExamContext>();

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
        app.MapRazorPages();
        app.Run();
    }
}
