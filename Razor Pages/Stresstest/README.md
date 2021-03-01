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
        var myservice = app.ApplicationServices.GetRequiredService<MyService>();
        var response = myservice.Method();
        // using Microsoft.AspNetCore.Http;
        await context.Response.WriteAsJsonAsync(response);
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

## Flexibler: Erstellen eines API Controllers

Wird in *app.UseEndpoints()* in *Startup.Configure()* der Aufruf *endpoints.MapControllers();*
eingebaut, kann auch ein klassischer API Controller zum Bearbeiten des Tests erstellt werden.
Dafür wird einfach ein Ordner *Controllers* und darin eine Klasse Klasse *StresstestController*
erstellt. Es kann mit normaler Dependency Injection gearbeitet werden, um die Requests
aus bombardier zu verarbeiten.

## Monitoring mit *dotnet counters*

Interessiert die Speicherauslastung, kann diese mit *dotnet-counters* abgerufen werden. XXX
ist durch den Namen der App aus *dotnet counters ps* zu ersetzen.

```text
dotnet tool install -g dotnet-counters
dotnet counters ps
dotnet counters monitor --name XXX System.Runtime Microsoft.AspNetCore.Hosting
```

Quelle: https://channel9.msdn.com/Shows/On-NET/ASPNET-Core-Series-Performance-Testing-Techniques
