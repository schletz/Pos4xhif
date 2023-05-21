# Lab 4 zu ASP.NET Core: Unittests

![](https://www.plantuml.com/plantuml/svg/jP7FIiGm4CRlVOhGexABNdlG1QjuK15Q7s1C1ZkQFoMPmehuxgOXgTZjrLvc-WtppMycXy3WUJAw6aYXG3Fofp38WrEXGiiKVti48xug4RzpyGG6HICwzcJVijR9mJajDOJmM_gkId_7auhfOd57Fh3Ty7c0RVtM0EcLSo6J0_h_S8RmiTXsq-ixIbutzyJwn34TgqaXAmMoALcPVHp90vEpBV3iCuUU3ERw8noV7LcURnh3TQHBLFN5VdzMYztkIjINTPgqMIZJoahtO1NPijJoAat9ifwicXIoSkCH6DKfOOj1UXhd5TPdQ9sSJ3HzyN_EcnbMf0LWkvf83cZLPFGF)

<small>[PlantUML Source](https://www.plantuml.com/plantuml/uml/jP7FIiGm4CRlVOhGexABNdlG1QjuK15Q7s1C1ZkQFoMPmehuxgOXgTZjrLvc-WtppMycXy3WUJAw6aYXG3Fofp38WrEXGiiKVti48xug4RzpyGG6HICwzcJVijR9mJajDOJmM_gkId_7auhfOd57Fh3Ty7c0RVtM0EcLSo6J0_h_S8RmiTXsq-ixIbutzyJwn34TgqaXAmMoALcPVHp90vEpBV3iCuUU3ERw8noV7LcURnh3TQHBLFN5VdzMYztkIjINTPgqMIZJoahtO1NPijJoAat9ifwicXIoSkCH6DKfOOj1UXhd5TPdQ9sSJ3HzyN_EcnbMf0LWkvf83cZLPFGF)</small>

Öffnen Sie als Basisimplementierung die Datei *[AspShowcase20230327.7z](AspShowcase20230327.7z)*.
Die Datenbank befindet sich nach Programmstart in der Datei *demo.db*.
Sie kann in DBeaver, ... betrachtet werden (SQLite Datenbank).

In der Implementierung befindet sich das Service *HandinService*.
Die Implementierung der einzelnen Methoden soll durch Unittests geprüft werden.
Erstellen Sie hierfür im Testprojekt eine Klasse *HandinServiceTests*.
Die Testklasse soll von *DatabaseTest* über Vererbung abgeleitet werden.

## Grundsätzliches zu Tests

Achten Sie darauf, dass Ihre Tests nicht von anderen Tests abhängig sind.
Es darf nicht sein, dass ein voriger Test einen Datenbestand schreibt, und ein zweiter Test verwendet diesen Datenbestand.
Beginnen Sie daher jede Testmethode mit einer leeren Datenbank.
Rufen Sie nur die zu testende Methode auf und verwenden keine andere Servicemethode.
Beispiel: Verwenden sie nicht *AddHandin* im Arrange Bereich, um danach das Verhalten von *EditHandin* zu prüfen.

## Schritt 1: Überarbeiten des Services

Im *HandinService* befinden sich noch Abfragen, die *DateTime.Now* verwenden.
Fügen Sie über Dependency Injection das schon vorhandene Interface *IClock* ein, um das Service testbar zu machen.

## Schritt 2: Erstellen der Tests für die Methode AddHandin

Legen Sie in der Testklasse *HandinServiceTests* folgende Testmethoden an:

- **AddHandinSuccessTest:** Prüft, ob ein Handin unter Regelbedingungen (Task nicht abgelaufen, ...) hinzugefügt werden kann.
- **AddHandinThrowsServiceExceptionIfStudentIsInvalidTest:** Wurde die übergebene Student GUID nicht gefunden, soll die Servicemethode eine *ServiceException* werfen. Prüfen Sie dieses Verhalten. Prüfen Sie auch die richtige Meldung in der Exception. Es könnte ja sein, dass die *ServiceException* aufgrund einer anderen Bedingung geworfen wird.
- **AddHandinThrowsServiceExceptionIfTaskIsInvalidTest:** Wurde die übergebene Task GUID nicht gefunden, soll die Servicemethode eine *ServiceException* werfen. Prüfen Sie dieses Verhalten. Prüfen Sie auch die richtige Meldung in der Exception. Es könnte ja sein, dass die *ServiceException* aufgrund einer anderen Bedingung geworfen wird.
- **AddHandinThrowsServiceExceptionIfTaskIsExpiredTest:** Ist der Task abgelaufen, soll die Servicemethode eine *ServiceException* werfen. Achten Sie beim Aufbau des Tests darauf, dass er deterministisch ist, also die aktuelle Systemzeit nirgends verwendet wird. Prüfen Sie auch die richtige Meldung in der Exception. Es könnte ja sein, dass die *ServiceException* aufgrund einer anderen Bedingung geworfen wird.

## Schritt 3: Erstellen der Tests für die Methode EditHandin und DeleteHandin

Überlegen Sie sich selbst die Testmethoden.
Welche Methoden brauchen Sie, um 100% Code Coverage zu erreichen?

## Schritt 4: Optimieren des Codes in der Testklasse

Der Code in der Testklasse wird vermutlich sehr repititiv (kopiert) sein.
Schreiben Sie eine generelle *GenerateDbFixtures* Methode, die Daten so in der Datenbank einfügt, dass alle Unittests damit durchgeführt werden können.
