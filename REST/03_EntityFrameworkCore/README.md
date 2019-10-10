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
Ziehe die Datei *Schüler.db* in den Solution Explorer über den Projektnamen. Dadurch wird die Datenbank
in das Projekt integriert. Mit folgendem Befehl in der Packet Manager Console kann ein Verzeichnis
Model2 erstellt und die Klassen generiert werden:
```powershell
Scaffold-DbContext "DataSource=Schule.db" Microsoft.EntityFrameworkCore.Sqlite -OutputDir Model2 -UseDatabaseNames
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

## CRUD Operationen mit Entity Framework Core
Mit dem Entity Framework Core können natürlich auch INSERT, UPDATE und DELETE Anweisungen abgesetzt werden.
Dafür wird der sogenannte EntityState eines Objektes in der DbSet Collection gesetzt.

![](entity-states.png)

<sup>Quelle: https://www.entityframeworktutorial.net/basics/entity-states.aspx</sup>

Folgendes Codebeispiel generiert einen Schüler mit einer zufälligen Schülernummer und fügt ihn in die
Datenbank ein. Die Klasse wird nur über die Navigation gesetzt, der Wert für *S_Klasse* wird nach dem 
Setzen des *EntityState* auf *Added* automatisch gesucht. Das anschließende *SaveChanges()* schickt
das *INSERT* an die Datenbank.
```c#
Random rnd = new Random();
Schueler s = new Schueler
{
    S_Nr = rnd.Next(),
    S_Geschl = "m",
    S_Zuname = "Mustermann",
    S_Vorname = "Max",
    S_KlasseNavigation = db.Klasse.Find("4AHIF")
};

db.Entry(s).State = Microsoft.EntityFrameworkCore.EntityState.Added;
try
{
    db.SaveChanges();
}
catch (Microsoft.EntityFrameworkCore.DbUpdateException)
{
    return StatusCode(StatusCodes.Status500InternalServerError);
}
```

Soll nun der Schüler aktualisiert werden, so wird das gewünschte Property geändert und der *EntityState*
auf *Modified* gesetzt. Ein *SaveChanges()* schreibt die *UPDATE* Anweisung.

```c#
s.S_Zuname = "Mustermann2";
db.Entry(s).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
try
{
    db.SaveChanges();
}
catch (Microsoft.EntityFrameworkCore.DbUpdateException)
{
    return StatusCode(StatusCodes.Status500InternalServerError);
}
```

Das Löschen eines Schülers wird durch das Setzen des *EntityState* auf *Deleted* gelöst:
```c#
db.Entry(s).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
try
{
    db.SaveChanges();
}
catch (Microsoft.EntityFrameworkCore.DbUpdateException)
{
    return StatusCode(StatusCodes.Status500InternalServerError);
}
```
 

## Übung
Öffne die Solution *PostRequestExample.sln* in diesem Ordner und verwende nun die Datenbank statt den Demodaten. Gehe dabei
so vor:
- Aktualisiere deinen Rechner auf VS 16.3 und .NET Core 3.
- Installiere Microsoft.EntityFrameworkCore.Tools und Microsoft.EntityFrameworkCore.Sqlite
- Generiere die Modelklassen zuerst in den Ordner *Model2*. Danach lösche den alten Ordner *Model* und
  benenne *Model2* auf *Model* um. Vergiss nicht, auch den Namespace umzubenennen. Achte auch auf die
  *[JsonIgnore]* Properties über den Navigationsproperties.
- Passe die Feldnamen an, so dass der Code korrekt ist.
- Implementiere die CRUD Operationen in den einzelnen Routen des Controllers.
