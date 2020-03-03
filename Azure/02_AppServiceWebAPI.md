# Erstellen einer ASP.NET WebAPI als App Service

In diesem Kapitel erstellen wir eine ASP.NET Core WebAPI, um unsere SQL Server Datenbank über eine 
REST Schnittstelle ansprechen zu können.

Dieses Beispiel benötigt .NET Core 3.1. Prüfe vorher in der Eingabeaufforderung, ob diese Version
auch installiert ist:

```text
...>dotnet --version
3.1.100
```

Falls nicht, führe ein Visual Studio Update aus. Die Version 16.4 aktualisiert auf .NET Core 3.1.
Wenn du kein Visual Studio verwendest (und nur dann), lade die neueste Version der .NET Core SDK
von https://dotnet.microsoft.com/download.

## Erstellen einer leeren WebAPI von der Konsole aus

Neue WebAPI Projekte können auch von der Konsole aus erstellt werden, indem ein neuer Ordner (z. B. *AzureDemo*)
erstellt wird. In diesem Ordner werden dann folgende Befehle ausgeführt. Der Connection String muss
allerdings noch an die eigenen Einstellungen angepasst werden.

```text
dotnet new webapi
dotnet tool update --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet ef dbcontext scaffold "Server=tcp:AAAA,1433;Initial Catalog=bbbb;Persist Security Info=False;User ID=cccc;Password=dddd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" Microsoft.EntityFrameworkCore.SqlServer --output-dir Model --use-database-names --force --data-annotations
```

Beim Verbindungsstring von scaffold sind folgende Dinge anzupassen:

- *Server=tcp:AAAA,1433*: Durch den Servernamen des SQL Servers zu ersetzen
- *Initial Catalog=bbbb*: Durch den Datenbanknamen  zu ersetzen
- *User ID=cccc*:  Benutzername des Datenbankusers.
- *Password=dddd*: Passwort des Datenbankusers.

Alternativ kann natürlich auch der von der SQL Datenbank kopierte Verbindungsstring (*Settings* >
*Connection string*) mit dem eingesetzten Passwort eingefügt werden.

### Erstellen einer WebAPI mit SQLite

Es ist auch möglich, eine SQLite Datenbank mit dem Projekt nach Azure zu übertragen. Der Vorteil dabei
ist, dass hier keine Kosten für die Datenbank anfallen.

```text
dotnet new webapi
dotnet tool update --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet ef dbcontext scaffold "DataSource=aaaa.db" Microsoft.EntityFrameworkCore.Sqlite --output-dir Model --use-database-names --force --data-annotations
```

Beim Verbindungsstring von scaffold sind folgende Dinge anzupassen:

- *DataSource=aaaa.db:* Durch den Datenbanknamen der SQLite Datei zu ersetzen.

> **Achtung:** Die Datenbank muss in beim Kompilieren ins Ausgabeverzeichnis kopiert werden, da sie
> sonst nicht übertragen wird. Dafür klickt man in Visual Studio auf die Datei im Solution Explorer
> und wählt statt *Do not copy* die Option *Copy always*. Dies kann auch händisch in der *.csproj*
> Datei eingetragen werden:

```xml
<ItemGroup>
  <None Update="xxx.db">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

Falls Probleme mit der Übertragung der Datenbank auftreten (Fehlermeldung *No such Table xxx*), kann
wie im Punkt *FTP Zugriff auf das App Service* beschrieben die Datenbank auch über FTP/S kopiert
werden.

## Hinzufügen eines Controllers

### ConfigureServices

Der DbContext wird mit *AddDbContext()* hinzugefügt. Über Dependency Injection wird dann eine Instanz
des Contextes im Konstruktor des Controllers von ASP.NET Core übergeben.

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();
    services.AddDbContext<AzureDemoDatabaseContext>();  // Entsprechendes using eintragen!
}
```

### Erstellen der Datei Person.cs in Controller

```c#
[Route("api/[controller]")]
[ApiController]
public class PersonController : ControllerBase
{
    private readonly AzureDemoDatabaseContext _context;
    public PersonController(AzureDemoDatabaseContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<Person> Get()
    {
        return Ok(_context.Person.AsEnumerable());
    }
}
```

## Publishing mit Visual Studio

### launchSettings.json

Die Datei *launchSettings.json* im Ordner Properties kann auf den folgenden Inhalt geändert werden.
Dadurch hört der Server auf den Standardports für HTTP und HTTPS.

```js
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "AzureDemoApp": {
      "commandName": "Project",
      "applicationUrl": "https://0.0.0.0:443;http://0.0.0.0:80",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}

```

### Publishing

Mit der rechten Maustaste kann in Visual Studio beim Projekt der Punkt *Publish* gewählt werden.
Auch hier ist darauf zu achten, dass das niedrigste Paket gewählt wird.

![](vs_publish_settings.png)

## Aktivieren des Development Profiles

Falls Fehler auftreten, werden keine Beschreibungen dazu in Azure angezeigt. Das liegt daran, dass diese
nur im Profil *Development* sichtbar sind. Um das zu aktivieren, klicke in https://portal.azure.com
unter *App Services* auf deine App. Unter den Einstellungen kann nun die Umgebungsvariable
*ASPNETCORE_ENVIRONMENT* auf *Development* gesetzt werden.

![](azure_settings.png)

## Aktivieren von MySQL in App

In Azure kann - ebenfalls unter *Settings* des App Serivce - der Punkt *MySQL in App* aktiviert
werden. Dabei wird ein Connectionstring angeboten, der in der Datei *Startup.cs* der ASP.NET
Core Applikation verwendet werden kann:

```c#
string connectionString = Environment.GetEnvironmentVariable("MYSQLCONNSTR_localdb")
    ?.Replace("Data Source", "Server")
    ?.Replace("User Id", "User") ?? "";             // Schöner: String aus appsettings.json statt ""
services.AddDbContext<ApplicationDbContext>(options =>  
    options.UseMySql(connectionString)
);
```

In Windows kann diese Variable *MYSQLCONNSTR_localdb* zum Testen angelegt werden (*Start* -
*Environment Variables*). Diese muss einen Connectionstring liefern, wie ihn auch Azure liefern würde.
Die einzelnen Werte können natürlich an das lokale System angepasst werden.

```text
Database=localdb;Data Source=127.0.0.1:50513;User Id=azure;Password=xxxxxx
```

Natürlich sind auch andere Varianten dieses Codes möglich. So kann z. B. wenn *GetEnvironmentVariable()*
null liefert, nicht der Leerstring sondern der String aus der Datei *appsettings.json* geladen werden.

## FTP Zugriff auf das App Service

Falls die Datenbank nicht korrekt angelegt wurde oder aus anderen Gründen ein Zugriff auf Dateiebene
notwendig ist, kann dies über FTP/S geschehen. Lade zuerst das Programm WinSCP von der
[Downloadseite](https://winscp.net/eng/downloads.php) und installiere das Programm.

Danach können die generierten FTP Zugangsdaten im App Service
(*https://portal.azure.com/* - *App Services*) der Applikation kopiert werden:

![](ftp_access.png)
