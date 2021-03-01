# Stresstest mit ASP.NET Core

## Direktes Testen von Services mit route-to-code in *Startup.Configure*

Um eine direkte Route ohne Controller oder Razor Page zu definieren, kann in der Methode
*UseEndpoints()* ein Service direkt aufgerufen und das Ergebnis als JSON an den Client geliefert
werden.

```c#
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();   // Existing code.
    endpoints.MapRazorPages();    // Existing code.

    // Route-to-code
    endpoints.MapGet("/stresstest", async context =>
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {        
            var myservice = scope.ServiceProvider.GetRequiredService<MyService>();
            var response = myservice.Method();
            // using Microsoft.AspNetCore.Http;
            await context.Response.WriteAsJsonAsync(response);
        }
    });
});
```

Nun wird der Server im Release Mode gestartet.

```text
dotnet run -c Release
```

## Senden von Anfragen mit bombardier

Auf https://github.com/codesenberg/bombardier/releases kann ein kleines Programm geladen werden, welches
den Server mit HTTP Requests bombardiert. Achtung: Der Browser und Windows Defender wird beim
Download mögliche Schadsoftware anzeigen, da das Programm eigentlich eine DoS Attacke durchführt.

Der folgende Aufruf öffnet 125 Verbindungen und sendet in Summe 10000 Anfragen an den Server:

```text
bombardier -c 125 -n 10000 http://localhost:5000/stresstest
```

In der Ausgabe ist abzulesen, dass der Server im Mittel 308 Requests pro Sekunde verarbeiten kann.

```text
bombardier -c 125 -n 10000 http://localhost:5000/stresstest

Bombarding http://localhost:5000 with 10000 request(s) using 125 connection(s)
 10000 / 10000 [====================================================================================] 100.00% 301/s 33s
Done!
Statistics        Avg      Stdev        Max
  Reqs/sec       307.82     614.12   11837.28
  Latency      412.60ms   105.11ms      1.08s
  HTTP codes:
    1xx - 0, 2xx - 10000, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:   223.13MB/s
```

## Flexibler: Erstellen eines eigenen Stresstest Projektes

Um nicht gegen die Produktionsdatenbank zu testen, legen wir für ein Projekt (z. B. unseren
TestAdministrator) ein eigenes Projekt an. Es ist ein leeres ASP.NET Core Projekt mit
Dependencies zu EF Core.

```text
md TestAdministrator.Stresstest
cd TestAdministrator.Stresstest
dotnet new web
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

Danach muss in Visual Studio die Solutiondaten geöffnet und das erstellte Projekt mittels
*Add* - *Existing Project* durch Rechtsklick auf die Solution hinzugefügt werden. Danach
wird im Projekt *TestAdministrator.Stresstest* unter *Dependencies* eine Referenz
auf das Webapp Projekt hinzugefügt.

### Anpassen des DbContext im Webapp Projekt

Im Hauptprojekt muss noch der DbContext angepasst werden, damit im Konstruktor eine
eigene Konfiguration übergeben werden kann.

```c#
public class ExamDatabase : DbContext
{
    public ExamDatabase(DbContextOptions options) : base(options) { }
    public ExamDatabase() : base() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Wichtig: Nur wenn nicht im Konstruktor definiert, dann gilt diese
        // Konfiguration.
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=Exams.db");
        }
    }
}
```

### Anpassen der Main Methode in TestAdministrator.Stresstest

Die Mainmethode erstellt die Musterdatenbank und verwendet die vorhandene Seed Methode, um
Musterdaten zu erzeugen. Natürlich kann auch eine eigene Seed Methode mit mehr Daten
verwendet werden.

```c#
public static void Main(string[] args)
{
    var options = new DbContextOptionsBuilder()
        .UseSqlite("Data Source=Stresstest.db")
        .Options;

    using (var db = new ExamDatabase(options))
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
        db.SeedDatabase();
    }

    CreateHostBuilder(args).Build().Run();
}
```

### route-to-code

Nach der Anpassung werden die Services des TestAdministrator Projektes registriert. Achte
auch auf die korrekte Konfiguration der Datenbank, damit auch auf die Testdatenbank
zugegriffen wird.

Danach wird eine Route */examservice/getallexams* definiert, die die Methode
*ExamService.GetAllExams()* aufruft.

```c#
public void ConfigureServices(IServiceCollection services)
{
    services.AddTransient<ExamService>();
    services.AddDbContext<ExamDatabase>(opt=>opt.UseSqlite("Data Source=Stresstest.db"));
}

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // ...
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/examservice/getallexams", async context =>
        {
            // Einen Scope erstellen, da das ExamService auch andere Services
            // benötigt.
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var examService = scope.ServiceProvider.GetRequiredService<ExamService>();
                var exams = examService.GetAllExams();
                // await context.Response.WriteAsJsonAsync(exams);
                context.Response.StatusCode = 200;
            }
        });
    });
}
```

### Durchführen des Tests

Nun kann mit *dotnet run -c Release* die Stresstest Applikation gestartet werden. Danach
wird mit bombardier die Route */examservice/getallexams* getestet.

```text
bombardier.exe -c 125 -n 10000 http://localhost:5000/examservice/getallexams

Bombarding http://localhost:5000/examservice/getallexams with 10000 request(s) using 125 connection(s)
 10000 / 10000 [==================================================================================] 100.00% 130/s 1m16s
Done!
Statistics        Avg      Stdev        Max
  Reqs/sec       161.31     569.00   12003.00
  Latency         0.95s   152.03ms      3.57s
  HTTP codes:
    1xx - 0, 2xx - 10000, 3xx - 0, 4xx - 0, 5xx - 0
    others - 0
  Throughput:    22.71KB/s
```

Wir sehen, dass diese Route zwar mit 161 Requests pro Sekunde sehr belastbar ist, jedoch die
Latenz bei **0.95 sec** liegt. Ein Benutzer muss also fast 1 Sekunde warten, bis das Service
überhaupt die Daten liefert.

Aktivieren wir die Zeile *await context.Response.WriteAsJsonAsync(exams);*, so kommt noch
das Zurückliefern der Daten als JSON mit ins Spiel. Mit dieser Option sinkt die Rate auf ca. 98
Requests/sec und die Latenz erhöht sich auf 1.38 sec.

## Monitoring mit *dotnet counters*

Interessiert die Speicherauslastung, kann diese mit *dotnet-counters* abgerufen werden. XXX
ist durch den Namen der App aus *dotnet counters ps* zu ersetzen.

```text
dotnet tool install -g dotnet-counters
dotnet counters ps
dotnet counters monitor --name XXX System.Runtime Microsoft.AspNetCore.Hosting
```

Quelle: https://channel9.msdn.com/Shows/On-NET/ASPNET-Core-Series-Performance-Testing-Techniques
