# Serverseitiges HTML mit ASP.NET Core Razor Pages

## Inhalt

- [Razor Pages 1 - Intro](01%20Intro/README.md)
- [Razor Pages 2 - Details Page](02%20Details%20Page/README.md)
- [Razor Pages 3 - Edit](03%20Edit%20und%20DTO/README.md)
- [Razor Pages 4 - Add](04%20Add/README.md)
- [Razor Pages 5 - Bulk Edit](05%20Bulk%20Edit/README.md)
- [Razor Pages 6 - Repository Pattern](06%20Repositories/README.md)

## 2 Strukturen - 1 Ziel

Mit ASP.NET Core kannst du serverseitig gerenderte Webapps auf 2 Arten erstellen:

### MVC (Model View Controller): dotnet new mvc

MVC Projekte haben 3 Ordner:

```
Webapp
  |
  +--- Controllers
  +--- ViewModels
  +--- Views
```

Der Controller ist der Endpunkt für das Routing. Mit *return View(viewmodel)* liefert er eine
Razor View an den Client aus, die das gerenderte HTML enthält.

### MVVM (Model - View - Viewmodel) mit Razor Pages: dotnet new webapp

Die Razor Pages Applikation besteht aus einer Razor View (die cshtml Datei) und einem entsprechenden
Viewmodel (cshtml.cs Datei). Das Viewmodel behandelt die Requests selbst, es ist also Controller und
(klassisches) Viewmodel in einem. Dadurch sind nur 2 Dateien notwendig:

```
Webapp
  |
  +--- Pages
         |
         +---- Index.cshtml
         +---- Index.cshtml.cs
```

