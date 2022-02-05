using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StoreManager.Application.Infrastructure;
using StoreManager.Application.Infrastructure.Repositories;
using StoreManager.Application.Services;
using StoreManager.Webapp.Dto;

// Erstellen und seeden der Datenbank
var opt = new DbContextOptionsBuilder()
    .UseSqlite("Data Source=stores.db")  // Keep connection open (only needed with SQLite in memory db)
    .Options;
using (var db = new StoreContext(opt))
{
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    db.Seed();
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlite("Data Source=stores.db")
        .EnableSensitiveDataLogging(true);
});
builder.Services.AddTransient<StoreRepository>();
builder.Services.AddTransient<OfferRepository>();
builder.Services.AddTransient<ProductRepository>();
builder.Services.AddTransient<ProductCategoryRepository>();
builder.Services.AddTransient<ProductImportService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddRazorPages();

// MIDDLEWARE
var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapRazorPages();
app.Run();
