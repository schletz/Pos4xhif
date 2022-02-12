using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StoreManager.Application.Dto;
using StoreManager.Application.Infrastructure;
using StoreManager.Application.Infrastructure.Repositories;
using StoreManager.Application.Model;
using StoreManager.Application.Services;
using StoreManager.Webapp.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// *************************************************************************************************
// ERZEUGEN UND INITIALISIEREN DER DATENBANK
// *************************************************************************************************
var opt = builder.Environment.IsDevelopment()
    ? new DbContextOptionsBuilder().UseSqlite(builder.Configuration.GetConnectionString("Sqlite")).EnableSensitiveDataLogging().Options
    : new DbContextOptionsBuilder().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")).Options;

using (var db = new StoreContext(opt))
{
    if (builder.Environment.IsDevelopment()) { db.Database.EnsureDeleted(); }
    if (db.Database.EnsureCreated())
    {
        db.Initialize(
            new CryptService(),
            Environment.GetEnvironmentVariable("STORE_ADMIN") ?? throw new ArgumentNullException("Die Variable STORE_ADMIN ist nicht gesetzt."));
    }
    if (builder.Environment.IsDevelopment()) { db.Seed(new CryptService()); }
}


// *************************************************************************************************
// SERVICES
// *************************************************************************************************
builder.Services.AddDbContext<StoreContext>(opt =>
{
    if (builder.Environment.IsDevelopment())
        opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")).EnableSensitiveDataLogging();
    else
        opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

// * Repositories **********************************************************************************
builder.Services.AddTransient<StoreRepository>();
builder.Services.AddTransient<OfferRepository>();
builder.Services.AddTransient<PriceTrendRepository>();
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
builder.Services.AddAntiforgery(o => o.HeaderName = "xsrf-token"); 
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddRazorPages();

// *************************************************************************************************
// MIDDLEWARE
// *************************************************************************************************

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// https://blog.elmah.io/the-asp-net-core-security-headers-guide/
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
    // https://wiki.selfhtml.org/wiki/Sicherheit/Content_Security_Policy
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data:");
    await next();
});

app.UseRequestLocalization(System.Globalization.CultureInfo.InvariantCulture.Name);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.Run();
