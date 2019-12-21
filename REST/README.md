# REST Webservices mit ASP.NET Core

Dieses Beispiele benötigen .NET Core 3.1. Prüfe vorher in der Eingabeaufforderung, ob diese Version
auch installiert ist:

```text
...>dotnet --version
3.1.100
```

Falls nicht, führe ein Visual Studio Update aus. Die Version 16.4 aktualisiert auf .NET Core 3.1.
Wenn du kein Visual Studio verwendest (und nur dann), lade die neueste Version der .NET Core SDK
von https://dotnet.microsoft.com/download.

## Anlegen eines ASP.NET Core WebAPI mit Visual Studio (Windows, macOS)

Um eine REST API mit ASP.NET Core zu erstellen, wird in Visual Studio 2019 mittels *Create a
new project* die Vorlage *ASP.NET Core Web Application* gewählt. Nach der Vergabe eines Namens wird
in den Projekteinstellungen der Punkt *API* gewählt. Die Checkbox bei *Configure for HTTPS* kann
auch angehakt bleiben. Es wird dann beim ersten Start ein Zertifikat generiert und eingetragen:

![](create_api_project2.png)


## Anlegen des Projektes von der Kommandozeile (Linux, macOS, Windows)

Wenn du auf Linux, macOS oder in der Konsole von Windows arbeiten möchtest, kannst du ein API
Projekt auch in der Eingabeaufforderung anlegen. Dafür erstelle zuerst ein Verzeichnis mit dem
Projektnamen und erstelle danach die Applikation darin.

```text
md GetRoutesDemo
cd GetRoutesDemo
dotnet new webapi
```

Es wird eine *.csproj* Datei angelegt, die mit Visual Studio oder Visual Studio Code geöffnet
werden kann. Sie hat folgenden Aufbau:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
  </PropertyGroup>
</Project>
```

Der Eintrag *TargetFramework* zeigt die SDK Version an. Um das Projekt kompilieren oder ausführen
zu können, muss diese SDK Version am System installiert sein.

### Starten des Servers

Im Ordner der Projektdatei kann einfach auf der Konsole mit *dotnet run* der Server gestartet werden.
Er zeigt den Port und das Netzwerkinterface aus der Konfiguration an.

Im Browser kann die Demoroute aus dem Template unter *http://localhost:5000/weatherforecast*
oder *https://localhost:5001/weatherforecast* angezeigt werden.

```text
...\GetRoutesDemo>dotnet run
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Users\Michael\Desktop\GetRoutesDemo
```

## Setzen der Ports und der Startadresse

Der Server hört standardmäßig auf die Ports 5000 und 5001. Um das zu ändern, ersetze die Datei
*Properties/launchSettings.json* durch folgenden Inhalt. Falls die Ports bei dir schon belegt sind,
können sie natürlich durch andere Werte ersetzt werden.

Wenn der Server von Visual Studio aus gestartet wird, wird auch das Property *launchUrl* ausgelesen.
Es gibt die Adresse an, die beim Öffnen des Browsers nach dem Serverstart erscheint.

```javascript
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "GetRoutesDemo": {
      "commandName": "Project",
      "launchBrowser": true,
      "launchUrl": "api/pupil",
      "applicationUrl": "https://localhost:443;http://localhost:80",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

Weitere Informationen:
- https://www.dpunkt.de/common/leseproben/12326/4_Kapitel%208.pdf
- https://www.infosys.com/digital/insights/Documents/restful-web-services.pdf
- https://github.com/Microsoft/api-guidelines/blob/master/Guidelines.md#6-client-guidance
