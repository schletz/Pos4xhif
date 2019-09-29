# POS im IV. Jahrgang der HTL Spengergasse

# Lehrinhalte auf Basis von Microsoft ASP.NET Core und XAMARIN Forms
Gem. Lehrplan BGBl. II Nr. 262/2015 für den 4. Jahrgang. Die kursiv gedruckten Teile in der Spalte Lehrplaninhalt 
kennzeichnen die wesentlichen Punkte im Sinne der LBVO.

Die Punkte unter Umsetzung betreffen den zweistündigen Teil des POS Unterrichtes. Der Rest wird im dreistündigen
Teil unterrichtet.

## Wintersemester im 4. Jahrgang

| Lehrplaninhalt                                                                                                                                                                                                         	| Umsetzung                                                                                              	| 
| -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	| -------------------------------------------------------------------------------------------------------	| 
| Plattformübergreifende Softwaresysteme erstellen:<br>Kommunikation zwischen heterogenen Systemen.                                                                                                                       	| REST Webservices in ASP.NET Core                                                                                                        	| 
| Fortgeschrittene Programmiertechniken anwenden:<br>Parallele Programmierung, Reflection, *objektorientierter Zugriff auf Massendaten*.                                                                                  	| Verwendung von EF Core in ASP.NET Core, asynchrones Laden der Daten.                                                                    	| 
| Software für unterschiedliche Plattformen erstellen:<br>*Entwicklungstechniken für unterschiedliche Plattformen*, Thread-Synchronisation, *Zugriff auf semi-strukturierte Daten*, *Build-Management*, Dokumentationsgenerierung.	| XAMARIN Forms: Views (Navigations, Liste, …), Persistenz                                                    	| 
| Die Konzepte von Programmiersprachen darlegen:<br>Einteilung und Eigenschaften von Programmiersprachen, Typsysteme, Programmierparadigmen.                                                                             	|                                                                                                                                         	| 
| Zusammenhänge von Problemstellungen erfassen und dafür einen umfassenden Entwurf der Struktur der Software erstellen:<br>Entwurfsrichtlinien, Strukturdiagramme, *Patterns*.                                             	|                                                                                                                                         	| 
| Komplexe, plattformübergreifende Softwaresysteme für den Produktivbetrieb erstellen:<br>Erstellung von Frameworks.                                                                                                      	|                                                                                                                                         	| 

### Leistungsbeurteilung
1 praktische LF aus dem Bereich REST Webservices sowie die Erstellung einer eigenen Aufgabenstellung im Bereich XAMARIN Forms oder Android Studio.

## Sommersemester 4. Jahrgang
| Lehrplaninhalt                                                                                                                                                                                                         	| Umsetzung                                                                                              	| 
| -----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	| -------------------------------------------------------------------------------------------------------	| 
| Plattformübergreifende Softwaresysteme erstellen:<br>Kommunikation zwischen heterogenen Systemen                                                                                                                       	| REST Webservices in Clients aufrufen.                                                                                                   	| 
| Fortgeschrittene Programmiertechniken anwenden:<br>Parallele Programmierung, Reflection, *objektorientierter Zugriff auf Massendaten*.                                                                                   	| XAMARIN Forms: Aufruf von REST Webservices, Persistenz (Synchronisation im Hintergrund)                                                  	| 
| Software für unterschiedliche Plattformen erstellen:<br>Entwicklungstechniken für unterschiedliche Plattformen, Thread-Synchronisation, Zugriff auf semi-strukturierte Daten, Build-Management, Dokumentationsgenerierung.	| XAMARIN Forms: gerätespezifischer Code (Android), Einbinden von fremden Komponenten, Erstellung von Controls, Wiederverwendung von Code.	| 
| Algorithmen nach Kriterien der Komplexität und Effizienz auswählen:<br>Komplexität von Algorithmen, Optimierung.                                                                                                       	|                                                                                                                                         	| 
| Systeme unter Berücksichtigung ihrer Dynamik analysieren und dafür einen umfassenden Entwurf des Verhaltens der Software erstellen:<br>Verhaltensdiagramme, *Patterns*.                                                  	|                                                                                                                                         	| 
| Software für den Produktivbetrieb erstellen:<br>Entwicklungstechniken für zuverlässige Systeme, Bug- und Issuetracking, Hilfesysteme, Integrationstests.                                                               	|                                                                                                                                         	| 

### Leistungsbeurteilung
1 praktische LF aus dem Bereich XAMARIN Forms sowie die Erstellung einer eigenen Aufgabenstellung im Bereich XAMARIN Forms oder Android Studio.


## Wichtiges zum Start:
1. [Installation von Visual Studio 2019](VisualStudioInstallation.md)
1. [Markdown Editing mit VS Code](https://github.com/schletz/Pos3xhif/blob/master/markdown.md)

## Synchronisieren des Repositories in einen Ordner
1. Lade von https://git-scm.com/downloads die Git Tools (Button *Download 2.xx for Windows*)
    herunter. Es können alle Standardeinstellungen belassen werden, bei *Adjusting your PATH environment*
    muss aber der mittlere Punkt (*Git from the command line [...]*) ausgewählt sein.
2. Lege einen Ordner auf der Festplatte an, wo du die Daten speichern möchtest 
    (z. B. *C:\Schule\POS\Examples*). Das
    Repository ist nur die lokale Version des Repositories auf https://github.com/schletz/Pos4xhif.git.
    Hier werden keine Commits gemacht und alle lokalen Änderungen dort werden bei der 
    nächsten Synchronisation überschrieben.
3. Initialisiere den Ordner mit folgenden Befehlen, die du in der Konsole in diesem Verzeichnis
    (z. B. *C:\Schule\POS\Examples*) ausführst:
```bash {.line-numbers}
git init
git remote add origin https://github.com/schletz/Pos4xhif.git
```

4. Lege dir in diesem Ordner eine Datei *syncGit.cmd* mit folgenden Befehlen an. 
    Durch Doppelklick auf diese Datei wird immer der neueste Stand geladen. Neu erstellte Dateien
    in diesem Ordner bleiben auf der Festplatte, geänderte Dateien werden allerdings durch 
    *git reset* auf den Originalstand zurückgesetzt.
```bash {.line-numbers}
git reset --hard
git pull origin master --allow-unrelated-histories
```


