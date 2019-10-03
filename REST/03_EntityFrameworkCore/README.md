# Entity Framework Core
Damit unser Webservice in eine Datenbank schreiben kann, arbeiten wir mit dem OR Mapper für .NET Core Anwendungen
Entity Framework Core. Die Grundfunktionen wie Navigation Properties und LINQ für den Zugriff sind ident,
deswegen ist eine Einarbeitung sehr leicht wenn bereits das Entity Framework .NET verwendet wurde.

## Arbeiten mit einer SQLite Datenbank
Als Datenbanksystem für den Unterricht verwenden wir SQLite, da sie alle Daten in einer Datei speichert
und diese leicht weitergegeben kann. SQLite Datenbanken können mit der Software [SQLite Studio](https://sqlitestudio.pl/index.rvt?act=download)
verwaltet werden. Diese Software gibt es auch in einer portable Version, sodass nur die ZIP Datei entpackt
und das Programm gestartet werden muss.

Unsere Datenbank *Schule.db* kann mit *Database* - *Add a Database* in SQLite Studio geladen werden. Danach
kann über *Connect to Database* in der Symbolleiste (1. Symbol) die Verbindung aufgebaut werden. Die Datenbank
besteht aus 2 Tabellen und einer Fremdschlüsselbeziehung:

![](class_diagram.png)

## Upgrade auf Visual Studio 16.3 und .NET Core 3
Um .NET Core zu nutzen, muss Visual Studio 2019 auf die Version 16.3 aktualisiert werden. Eine Anleitung
ist auf [Visual Studio Docs](https://docs.microsoft.com/en-us/visualstudio/install/update-visual-studio?view=vs-2019)
zu finden.

Das Ausgangsprojekt in diesem Ordner ist bereits für .NET Core 3 eingerichtet. Es ist daher nicht mehr
möglich, ein Downgrade auf .NET Core 2.1 oder 2.2 durchzuführen, da sich die Konfigurationsoptionen von
ASP.NET Core 3.0 geändert haben. Möchte man das Projekt händisch aktualisieren, so müssen die Dateien
*Program.cs* und *Startup.cs* angepasst werden. Angepasste Versionen sind in diesem Ordner zu finden.

## Installation von EntityFramework.Core
Wir benötigen 2 Pakete, um auf die SQLite Datenbank zugreifen zu können:
- Microsoft.EntityFrameworkCore.Tools 
- Microsoft.EntityFrameworkCore.Sqlite

Diese können entweder über die grafische Oberfläche von NuGet oder über die Packet Manager Console
installiert werden. Diese kann über das Menü *Tools* - *NuGet Package Manager* - *Packet Manager Console*
geöffnet werden. Für die Installation sind 2 Befehle einzugeben:
```powershell
Install-Package Microsoft.EntityFrameworkCore.Tools 
Install-Package Microsoft.EntityFrameworkCore.Sqlite
```

## Automatisches Erstellen der Modelklassen
Ziehe die Datei Schüler.db in den Solution Explorer über den Projektnamen. Dadurch wird die Datenbank
in das Projekt integriert. Mit folgendem Befehl in der Packet Manager Console kann ein Verzeichnis
Model2 erstellt und die Klassen generiert werden:
```powershell
Scaffold-DbContext "DataSource=../Schule.db" Microsoft.EntityFrameworkCore.Sqlite -OutputDir Model2 -UseDatabaseNames
```

**Achtung: Für diesen Vorgang muss das Projekt erstellt werden können. Syntaxfehler, die z. B. durch
das Löschen des Model Ordners entstehen, führen zu einem Fehler!**

Nun kann im Controller die Datenbank instanziert und wie gewohnt mit LINQ abgefragt werden:
```c#
private readonly SchuleContext db = new SchuleContext();
[HttpGet]
public IActionResult Get()
{
    return Ok(db.Schueler.Select(s => new { s.S_Vorname, s.S_Zuname }));
}
```

## Übung
Öffne die Solution *PostRequestExample.sln* in diesem Ordner und verwende nun die Datenbank statt den Demodaten. Gehe dabei
so vor:
- Aktualisiere deinen Rechner auf VS 16.3 und .NET Core 3.
- Installiere Microsoft.EntityFrameworkCore.Tools und Microsoft.EntityFrameworkCore.Sqlite
- Generiere die Modelklassen zuerst in den Ordner *Model2*. Danach lösche den alten Ordner *Model* und
  benenne *Model2* auf *Model* um. Vergiss nicht, auch den Namespace umzubenennen.
- Passe die Feldnamen an, so dass der Code korrekt ist.
