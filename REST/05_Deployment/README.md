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
die auf einem anderen Port als die HTTP Adresse hört:
```js
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "PostRequestExample.App": {
      "commandName": "Project",
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
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

## Erstellen des Release Builds
Der folgende Befehl erzeugt einen Releasebuild mit folgenden Optionen:
- publish: Fügt alle Abhängigkeiten hinzu.
- -c Release: Erzeugt einen Release Build.
- -o (Dir): Speichert die Dateien im angegebenen Verzeichnis.
- --self-contained: Fügt die .NET Runtimebibliotheken hinzu.
- -r winx64 Gibt die Zielplattform an. Eine Auflistung ist im [RID Catalog](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog)
  zu finden.
```
...\AuthExample.App>dotnet publish -c Release -o ../Production --self-contained -r win-x64
```

## Ausführen der Datei
Der oben erzeugte Ordner Production wird auf das Zielsystem kopiert. Dort kann mit folgendem Befehl
der Server gestartet werden:
```
.../Production>dotnet AuthExample.App.dll
```

Soll der Port nachträglich geändert werden, so kann in der Datei *appsettings.json* durch Hinzufügen
der folgenden Optionen der Port geändert werden:
```js
  "Kestrel": {
    "EndPoints": {
      "Http": {
        "Url": "http://0.0.0.0:5002"
      }
    }
  }
```

Eine Auflistung aller Konfigurationsmöglichkeiten ist in der [MSDN](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel?view=aspnetcore-3.0) verfügbar.

Natürlich müssen die verwendeten Datenbanken und externe Ressourcen auch am Zielrechner zur Verfügung
stehen. Dafür ist ggf. die Datei *appsettings.json* an die veränderten Connection Strings, etc. anzupassen.