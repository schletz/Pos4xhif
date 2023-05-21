# Lab 1 zu ASP.NET Core: GET Methoden Operationen in Controllern

![](https://www.plantuml.com/plantuml/svg/jP7FIiGm4CRlVOhGexABNdlG1QjuK15Q7s1C1ZkQFoMPmehuxgOXgTZjrLvc-WtppMycXy3WUJAw6aYXG3Fofp38WrEXGiiKVti48xug4RzpyGG6HICwzcJVijR9mJajDOJmM_gkId_7auhfOd57Fh3Ty7c0RVtM0EcLSo6J0_h_S8RmiTXsq-ixIbutzyJwn34TgqaXAmMoALcPVHp90vEpBV3iCuUU3ERw8noV7LcURnh3TQHBLFN5VdzMYztkIjINTPgqMIZJoahtO1NPijJoAat9ifwicXIoSkCH6DKfOOj1UXhd5TPdQ9sSJ3HzyN_EcnbMf0LWkvf83cZLPFGF)

<small>[PlantUML Source](https://www.plantuml.com/plantuml/uml/jP7FIiGm4CRlVOhGexABNdlG1QjuK15Q7s1C1ZkQFoMPmehuxgOXgTZjrLvc-WtppMycXy3WUJAw6aYXG3Fofp38WrEXGiiKVti48xug4RzpyGG6HICwzcJVijR9mJajDOJmM_gkId_7auhfOd57Fh3Ty7c0RVtM0EcLSo6J0_h_S8RmiTXsq-ixIbutzyJwn34TgqaXAmMoALcPVHp90vEpBV3iCuUU3ERw8noV7LcURnh3TQHBLFN5VdzMYztkIjINTPgqMIZJoahtO1NPijJoAat9ifwicXIoSkCH6DKfOOj1UXhd5TPdQ9sSJ3HzyN_EcnbMf0LWkvf83cZLPFGF)</small>

Öffnen Sie als Basisimplementierung die Datei *[AspShowcase20230220.7z](AspShowcase20230220.7z)*.
Die Datenbank befindet sich nach Programmstart in der Datei *demo.db*.
Sie kann in DBeaver, ... betrachtet werden (SQLite Datenbank).

## Aufgaben

- Verwenden Sie 2 Klassen, die mit 1:n verbunden sind, aus Ihrem POS EF Core Projekt aus dem 5. Semester.
- Kopieren Sie diese Klassen in den Ordner *Models*.
    Beispiel: Project und Tasks, 1 Project hat mehrere Tasks.
- Ergänzen Sie die *Seed()* Methode, sodass Sie Musterdaten generieren.
- Schreiben Sie einen Controller für "die 1er Seite", in diesem Beispiel *Project*.
- Unter GET */api/projects* (bei Ihnen entsprechend anders) soll eine Liste an Projekten ausgegeben werden. Die Tasks sollen aber nicht im JSON enthalten sein.
- Verwenden Sie *Select()* in LINQ, um die Daten entsprechend zu projizieren. Beispiel: `_db.Tasks.Select(t=>new {t.Feld1, t.Feld2}).ToList()`
- Ergänzen Sie eine Route für */api/projects/3*, (bei Ihnen entsprechend anders), die alle Infos zum Projekt mit der ID 3 liefert.
- Verwenden Sie dafür [HttpGet("{id}"] und übergeben die ID als Methodenparameter.
