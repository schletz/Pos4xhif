# Deployment von ASP.NET Applikationen

Soll der Server für ein Projekt in den Produktivbetrieb gehen, sind folgende Schritte zu erledigen:

- Anpassung der Datei *launchSettings.json*.
- Erstellen des Release Builds.
- Ausführen am Zielrechner.

## Anpassen der Konfiguration

Die von Visual Studio erstelle Konfiguration startet den IIS Express Webserver. Um den Kestrel Server
direkt zu starten, werden die IIS Profile in der Datei *Properties/launchSettings.json* entfernt. Weiters
kann der verwendete Port (hier 5000) angepasst werden. Die Datei sieht nach der Entfernung des IIS
Profiles so aus:

```js
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "PostRequestExample.App": {
      "commandName": "Project",
      "applicationUrl": "http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

Die gesetzte Information unter *ASPNETCORE_ENVIRONMENT* kann in der ASP.NET Core Applikation über
Dependency Injection mit dem IHostingEnvironment Interface abgefragt werden. Eine Beschreibung ist auf der
[MSDN](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-3.0)
verfügbar.

## SSL verwenden

Gerade bei der Übertragung von Passwörtern muss unser Server die Daten über das https Protokoll
verschlüsseln. Dabei gibt es 2 Möglichkeiten:

- Verwenden eines vertrauenswürdigen SSL Zertifikates.
- Generieren eines selbst signierten SSL Zertifikates.

Die Verschlüsselungsalgorithmen und somit die Sicherheit der Verschlüsselung selbst sind bei beiden 
Varianten gleich, bei selbst signierten Zertifikaten erscheint im Browser jedoch eine Warnmeldung und 
ist somit nur für Testzwecke geeignet.

### Anpassen der Datei *launchSettings.json*

Wir ergänzen bei *applicationUrl* in der Datei *Properties/launchSettings.json* einfach eine HTTPS Adresse,
die auf den Standardport für HTTPS (443) hört. Außerdem schreiben wir statt localhost das Interface
0.0.0.0, da sonst Verbindungen über das Netzwerk nicht angenommen werden.

```js
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "PostRequestExample.App": {
      "commandName": "Project",
      "applicationUrl": "http://0.0.0.0:80;https://0.0.0.0:443",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

Falls ein Benutzer die http Adresse ohne SSL aufrufen möchte, können wir ihn über die Methode
*UseHttpsRedirection()* in der Datei *Startup.cs* auf die https Seite umleiten. Diese Methode muss
gleich am Anfang aufgerufen werden:

```c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseHttpsRedirection();
    // Other Code...
}
```

> **Achtung**: Dieses Zertifikat ist - wie oben erwähnt - selbst signiert und daher nicht vertrauenswürdig.
> Für Produktionsanwendungen muss ein vertrauenswürdiges Zertifikat vom Hoster oder einem unabhängigen
> Anbieter erworben werden.

## Setzen der Ports

Der Server hört standardmäßig nur auf das Loopback Interface. Wir haben zwar *launchSettings.json*
geändert, dies greift jedoch nur bei *dotnet run*. Um diese Konfiguration auch beim Publishing
zu ändern, fügt man in der Datei *Program.cs* in die Methode *CreateHostBuilder()* die Anweisung
*UseUrls()* ein. Der Stern bedeutet, dass jede Netzwerkschnittstelle abgehört wird.

```c#
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseUrls("http://*:80;https://*.443")
                      .UseStartup<Startup>();
        });
```

## Erstellen des Release Builds

Der folgende Befehl erzeugt einen Releasebuild mit folgenden Optionen:

- publish: Fügt alle Abhängigkeiten hinzu.
- -c Release: Erzeugt einen Release Build.
- -o (Dir): Speichert die Dateien im angegebenen Verzeichnis.
- --self-contained: Fügt die .NET Runtimebibliotheken hinzu.
- -r winx64 Gibt die Zielplattform an. Eine Auflistung ist im [RID Catalog](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog)
  zu finden.

```text
...\AuthExample.App>dotnet publish -c Release -o ../Production -r win-x64 --self-contained
```

### Erstellen einer einzelnen exe Datei

Mit .NET Core 3 ist es auch möglich, das gesamte Framework samt Dependencies in eine einzelne exe
Datei (self-contained single executable) zu packen. Diese hat rd. 100 MB, dafür kann sie auf jedem
Rechner ausgeführt werden.

```text
...\AuthExample.App>dotnet publish -c Release -o ../Production -r win-x64 /p:PublishSingleFile=true
```

Ein experimentelles Feature ist der zusätzliche Parameter */p:PublishTrimmed=true*. Es reduziert die
Größe der exe Datei. Diese Option ist allerdings noch experimentell und sollte noch nicht verwendet
werden. Details sind auf https://dotnetcoretutorials.com/2019/06/27/the-publishtrimmed-flag-with-il-linker/
erklärt.

## Ausführen der Datei

Der oben erzeugte Ordner Production wird auf das Zielsystem kopiert. Dort kann mit folgendem Befehl
der Server gestartet werden:

```text
.../Production>dotnet AuthExample.App.dll
```

Eine Auflistung aller Konfigurationsmöglichkeiten ist in der [MSDN](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-3.0) verfügbar.

Natürlich müssen die verwendeten Datenbanken und externe Ressourcen auch am Zielrechner zur Verfügung
stehen. Dafür ist ggf. die Datei *appsettings.json* an die veränderten Connection Strings, etc. anzupassen.
