using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StoreManager.Application.Dto;
using StoreManager.Application.Infrastructure;
using StoreManager.Application.Infrastructure.Repositories;
using StoreManager.Application.Model;
using StoreManager.Application.Services;
using StoreManager.Webapp.Services;

// Erstellen und seeden der Datenbank
var opt = new DbContextOptionsBuilder()
    .UseSqlite("Data Source=stores.db")
    .Options;
using (var db = new StoreContext(opt))
{
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
    db.Seed(new CryptService());
}

var builder = WebApplication.CreateBuilder(args);
// *************************************************************************************************
// SERVICES
// *************************************************************************************************
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlite("Data Source=stores.db")
        .EnableSensitiveDataLogging(true);
});

// * Repositories **********************************************************************************
builder.Services.AddTransient<StoreRepository>();
builder.Services.AddTransient<OfferRepository>();
builder.Services.AddTransient<UserRepository>();
builder.Services.AddTransient<ProductRepository>();
builder.Services.AddTransient<ProductCategoryRepository>();
builder.Services.AddTransient<ProductImportService>();

// * Services for authentication *******************************************************************
// To access httpcontext in services
builder.Services.AddHttpContextAccessor();
// Hashing methods
builder.Services.AddTransient<ICryptService, CryptService>();
builder.Services.AddTransient<AuthService>(provider => new AuthService(
    isDevelopment: builder.Environment.IsDevelopment(),
    db: provider.GetRequiredService<StoreContext>(),
    crypt: provider.GetRequiredService<ICryptService>(),
    httpContextAccessor: provider.GetRequiredService<IHttpContextAccessor>()));
builder.Services.AddAuthentication(
    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/User/Login";
        o.AccessDeniedPath = "/User/AccessDenied";
    });
builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("OwnerOrAdminRole", p => p.RequireRole(Usertype.Owner.ToString(), Usertype.Admin.ToString()));
});


// * Other Services ********************************************************************************
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddRazorPages();

// *************************************************************************************************
// MIDDLEWARE
// *************************************************************************************************

var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
