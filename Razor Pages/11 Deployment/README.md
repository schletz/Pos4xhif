# Razor Pages 11 - Depolyment

## Inhalt

- ASP.NET Core Environments für Production und Development
- Unterschiedliche Datenbanken für Production und Deployment
- Umgebungsvariablen für Initialkennwörter
- Erstellen einer Azure SQL Server Datenbank
- Veröffentlichen der App als Azure App Service
- Content Security Policy

Das Video ist auf https://youtu.be/Bfu_poXuL_o verfügbar (53min). Der Programmcode ist im
Ordner [StoreManager](StoreManager) zu finden.

Informationen über Docker Images von Datenbanksystemen finden Sie im Kapitel EF Core des Kurses
Pos3xhif: https://github.com/schletz/Pos3xhif/blob/master/03%20EF%20Core/07_DatabaseFirst/Docker.md

**startServer.cmd**

```
cd StoreManager.Webapp
SET STORE_ADMIN=EinAdminPasswort
SET ASPNETCORE_ENVIRONMENT=Development
dotnet watch run
```

**Program.cs 1: Datenbank initialisieren**

Löschen & Seed in Development, Erstellen der Tabellen ohne Löschen der Datenbank in Production.
Das Kennwort wird aus der Umgebungsvariable *STORE_ADMIN* gelesen. Die Methode
*Initialize* legt Grunddaten in der neu erstellten Datenbank 1x an. In diesem Fall ist dies der Admin User.

```c#
var builder = WebApplication.CreateBuilder(args);
var opt = builder.Environment.IsDevelopment()
    ? new DbContextOptionsBuilder().UseSqlite(builder.Configuration.GetConnectionString("Sqlite")).EnableSensitiveDataLogging().Options
    : new DbContextOptionsBuilder().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")).Options;

using (var db = new StoreContext(opt))
{
    if (builder.Environment.IsDevelopment()) { db.Database.EnsureDeleted(); }
    // Creating the tables when the database is empty or not present. 
    if (db.Database.EnsureCreated())           // Initialize only 1 time.
    {
        db.Initialize(
            new CryptService(),
            Environment.GetEnvironmentVariable("STORE_ADMIN") ?? throw new ArgumentNullException("Die Variable STORE_ADMIN ist nicht gesetzt."));
    }
    if (builder.Environment.IsDevelopment()) { db.Seed(new CryptService()); }
}
```

**Program.cs 2: Datenbankservice registrieren**

```c#
builder.Services.AddDbContext<StoreContext>(opt =>
{
    if (builder.Environment.IsDevelopment())
        opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")).EnableSensitiveDataLogging();
    else
        opt.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
```

**Program.cs 3: Benutzerdefinierte Error Page und Security Header**

```c#
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
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; script-src 'self' 'unsafe-inline'; style-src 'self' 'unsafe-inline'; img-src 'self' data:");
    await next();
});
```
