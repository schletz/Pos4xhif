# Lab 6 zu ASP.NET Core: Add, Delete und Edit Page

![](https://www.plantuml.com/plantuml/svg/jP7FIiGm4CRlVOhGexABNdlG1QjuK15Q7s1C1ZkQFoMPmehuxgOXgTZjrLvc-WtppMycXy3WUJAw6aYXG3Fofp38WrEXGiiKVti48xug4RzpyGG6HICwzcJVijR9mJajDOJmM_gkId_7auhfOd57Fh3Ty7c0RVtM0EcLSo6J0_h_S8RmiTXsq-ixIbutzyJwn34TgqaXAmMoALcPVHp90vEpBV3iCuUU3ERw8noV7LcURnh3TQHBLFN5VdzMYztkIjINTPgqMIZJoahtO1NPijJoAat9ifwicXIoSkCH6DKfOOj1UXhd5TPdQ9sSJ3HzyN_EcnbMf0LWkvf83cZLPFGF)

<small>[PlantUML Source](https://www.plantuml.com/plantuml/uml/jP7FIiGm4CRlVOhGexABNdlG1QjuK15Q7s1C1ZkQFoMPmehuxgOXgTZjrLvc-WtppMycXy3WUJAw6aYXG3Fofp38WrEXGiiKVti48xug4RzpyGG6HICwzcJVijR9mJajDOJmM_gkId_7auhfOd57Fh3Ty7c0RVtM0EcLSo6J0_h_S8RmiTXsq-ixIbutzyJwn34TgqaXAmMoALcPVHp90vEpBV3iCuUU3ERw8noV7LcURnh3TQHBLFN5VdzMYztkIjINTPgqMIZJoahtO1NPijJoAat9ifwicXIoSkCH6DKfOOj1UXhd5TPdQ9sSJ3HzyN_EcnbMf0LWkvf83cZLPFGF)</small>

Öffnen Sie als Basisimplementierung Ihre **Lösung zu Lab 5 (Index und Details Page)**, wo Sie eine Auflistung aller Tasks implementiert haben.
Die Datenbank befindet sich nach Programmstart in der Datei *demo.db*.
Sie kann in DBeaver, ... betrachtet werden (SQLite Datenbank).

## Anlegen von Tasks (Tasks/Add.cshtml)

- Legen Sie eine leere Razor Page *Tasks/Add.cshtml* an und laden Sie das *TaskService* über Dependency Injection.
  Um auf die Datenbank lesend zuzugreifen können Sie entweder den Datenbankcontext zusätzlich laden oder im *TaskService* ein Property *Tasks* erstellen, dass *Set\<Task\>* als IQueryable zurückgibt.
- Um auf die Page *Tasks/Add.cshtml* zu gelangen, fügen Sie am Start der Taskauflistung in *Tasks/Index* einen Link mit dem Titel *Add new task* hinzu.
  Verwenden Sie entsprechende Taghelper zur Generierung des Links.
- Bieten Sie Formularfelder für alle Felder der schon bestehenden Klasse *NewTaskCmd* (Application Projekt, Ordner Commands) an.
  - Verwenden Sie Dropdowns (*select* Element in HTML) für die Fremdschlüsselwerte von Team und Teacher GUID.
- Verwenden Sie die Methode *AddTask* im *TaskService*, um den Datensatz hinzuzufügen.
- Tritt eine *ServiceException* auf, zeigen Sie diese unter dem HTML Formular an.
  Die Werte im Formular sollen natürlich erhalten bleiben, damit der User sie korrigieren kann.
- Achten Sie darauf, dass die Validierung in der Klasse *NewTaskCmd* auch geprüft wird.
  Treten Validierungsfehler auf, sind diese dem User unter dem entsprechenden Formularfeld rückzumelden.
- Bei erfolgreicher Speicherung leiten Sie auf die Page *Tasks/Index* weiter.

## Löschen von Tasks (Tasks/Delete.cshtml)

- Legen Sie eine leere Razor Page *Tasks/Delete.cshtml* an und laden Sie das *TaskService* über Dependency Injection.
  Um auf die Datenbank lesend zuzugreifen können Sie entweder den Datenbankcontext zusätzlich laden oder im *TaskService* ein Property *Tasks* erstellen, dass *Set\<Task\>* als IQueryable zurückgibt.
- Fügen Sie zur Seite *Tasks/Index* eine Spalte *Actions* zu den Taskinformationen hinzu.
- Um auf die Page *Tasks/Delete.cshtml* zu gelangen, fügen Sie bei jedem Task in der Actions Spalte einen Link mit dem Titel *Delete* ein.
- Beim Klicken auf den Link *Delete* wird die Page *Tasks/Delete* mit der richtigen GUID des Tasks (Routing Parameter) geöffnet.
  Verwenden Sie entsprechende Taghelper zur Generierung des Links.
- Fügen Sie eine Sicherheitsabfrage ein.
  Es soll nochmals bestätigt werden, ob der Task gelöscht werden soll.
  Bei der Sicherheitsfrage soll der Titel des Tasks noch einmal angezeigt werden.
- Klickt der User auf Ja, wird der Task im *OnPostYes* Pagehandler gelöscht:
  - Verwenden Sie die Methode *DeleteTask* im Service *TaskService*.
    Der Parameter *force* wird auf *false* gesetzt.
  - Fangen Sie die *ServiceException* ab.
    Tritt ein Fehler auf (der Task hat Handins), so soll auf die Indexpage zurückgeroutet und diese Fehlermeldung angezeigt werden.
    Verwenden Sie hierfür *TempData*, um die Meldung auf der Indexpage der Tasks anzeigen zu können.
- Klickt der User auf Nein, wird im Pagehandler *OnPostNo* auf die Indexpage *Tasks/Index* weitergeleitet.

## Editieren von Tasks (Tasks/Edit.cshtml)

- Legen Sie eine leere Razor Page *Tasks/Edit.cshtml* an und laden Sie das *TaskService* über Dependency Injection.
  Um auf die Datenbank lesend zuzugreifen können Sie entweder den Datenbankcontext zusätzlich laden oder im *TaskService* ein Property *Tasks* erstellen, dass *Set\<Task\>* als IQueryable zurückgibt.
- Um auf die Page *Tasks/Edit.cshtml* zu gelangen, fügen Sie bei jedem Task in der Actions Spalte einen Link mit dem Titel *Edit* ein.
- Beim Klicken auf den Link *Edit* wird die Editpage mit der richtigen GUID des Tasks (Routing Parameter) geöffnet.
  Verwenden Sie entsprechende Taghelper zur Generierung des Links.
- Bieten Sie Formularfelder für alle Felder der schon bestehenden Klasse *EditTaskCmd* (Application Projekt, Ordner Commands) an.
  Achten Sie darauf, dass die Felder schon mit den entsprechenden Werten aus der Datenbank befüllt sind.
- Verwenden Sie die Methode *EditTask* im *TaskService*, um den Datensatz zu ändern.
- Tritt eine *ServiceException* auf, zeigen Sie diese unter dem HTML Formular an.
  Die Werte im Formular sollen natürlich erhalten bleiben, damit der User sie korrigieren kann.
- Achten Sie darauf, dass die Validierung in der Klasse *EditTaskCmd* auch geprüft wird.
  Treten Validierungsfehler auf, sind diese dem User unter dem entsprechenden Formularfeld rückzumelden.
- Bei erfolgreicher Speicherung leiten Sie auf die Page *Tasks/Index* weiter.

