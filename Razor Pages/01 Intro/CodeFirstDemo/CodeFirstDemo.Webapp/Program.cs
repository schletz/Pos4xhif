using CodeFirstDemo.Application.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var opt = new DbContextOptionsBuilder()
    .UseSqlite("Data Source=Stores.db")
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
    opt.UseSqlite("Data Source=Stores.db");
});
builder.Services.AddRazorPages();

var app = builder.Build();

// MIDDLEWARE
app.UseStaticFiles();
app.MapRazorPages();
app.Run();
