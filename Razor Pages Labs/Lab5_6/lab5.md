# Lab 5 zu ASP.NET Core: Details Page

![](https://www.plantuml.com/plantuml/svg/jP7FIiGm4CRlVOhGexABNdlG1QjuK15Q7s1C1ZkQFoMPmehuxgOXgTZjrLvc-WtppMycXy3WUJAw6aYXG3Fofp38WrEXGiiKVti48xug4RzpyGG6HICwzcJVijR9mJajDOJmM_gkId_7auhfOd57Fh3Ty7c0RVtM0EcLSo6J0_h_S8RmiTXsq-ixIbutzyJwn34TgqaXAmMoALcPVHp90vEpBV3iCuUU3ERw8noV7LcURnh3TQHBLFN5VdzMYztkIjINTPgqMIZJoahtO1NPijJoAat9ifwicXIoSkCH6DKfOOj1UXhd5TPdQ9sSJ3HzyN_EcnbMf0LWkvf83cZLPFGF)

<small>[PlantUML Source](https://www.plantuml.com/plantuml/uml/jP7FIiGm4CRlVOhGexABNdlG1QjuK15Q7s1C1ZkQFoMPmehuxgOXgTZjrLvc-WtppMycXy3WUJAw6aYXG3Fofp38WrEXGiiKVti48xug4RzpyGG6HICwzcJVijR9mJajDOJmM_gkId_7auhfOd57Fh3Ty7c0RVtM0EcLSo6J0_h_S8RmiTXsq-ixIbutzyJwn34TgqaXAmMoALcPVHp90vEpBV3iCuUU3ERw8noV7LcURnh3TQHBLFN5VdzMYztkIjINTPgqMIZJoahtO1NPijJoAat9ifwicXIoSkCH6DKfOOj1UXhd5TPdQ9sSJ3HzyN_EcnbMf0LWkvf83cZLPFGF)</small>

Öffnen Sie als Basisimplementierung die Datei *[AspShowcase20230417.7z](AspShowcase20230417.7z)*.
Die Datenbank befindet sich nach Programmstart in der Datei *demo.db*.
Sie kann in DBeaver, ... betrachtet werden (SQLite Datenbank).

## Anzeigen der Teams und Tasks

Erstellen Sie im Ordner *Pages* einen Unterordner *Tasks*.
In diesem Ordner *Tasks* erstellen Sie eine leere Razor Page mit dem Namen *Index.cshtml*.
Die Seite soll folgenden Aufbau haben:
- Fügen Sie die Datenbank über Dependency Injection ein.
- Als Zwischenüberschrift (h4 Element) sollen die in der Datenbank vorhandenen Teams mit dem Namen ausgegeben werden.
- Sortieren Sie nach dem Teamnamen.
- Geben Sie pro Team die Tasks mit den folgenden Informationen aus:
    - Titel des Tasks
    - Fälligkeit (Expiration)
    - Vor- und Zuname des Lehrers
    - Max. Punkte (*MaxPoints*)
    - Anzahl der eingelangten Abgaben (Handins)
- Hat ein Team keine Tasks, soll nur die Information *keine Tasks* ausgegeben werden.
- Noch offene Tasks (Expiration Date ist in der Zukunft) sollen hervorgehoben werden, definieren Sie einen geeigneten Style hierfür.
- Wenn nötig erstellen Sie Navigations in den Modelklassen, damit Sie leichter die Darstellung erzeugen können.
- Verwenden Sie DTO Klassen im PageModel, um die Informationen auszugeben.
- Die Anzahl der eingelangten Abgaben ist ein Link.
  Klickt ein User auf diesen Link, soll er auf die Detailpage des Tasks kommen (siehe nächster Punkt).

Fügen Sie in der Navigation Bar einen Link zu der Seite */tasks/index* ein.

## Detailpage eines Tasks

Unter */tasks/details/(guid)* soll eine Page mit den Details zum aktuellen Task ausgegeben werden.
Geben Sie die Grunddaten samit Team- und Lehrername im oberen Seitenbereich aus.
Listen Sie danach alle Abgaben (Handins) mit Studentname, Description und URL als Tabelle.
