# Lab 3 zu ASP.NET Core: Services

![](https://www.plantuml.com/plantuml/svg/jP7FIiGm4CRlVOhGexABNdlG1QjuK15Q7s1C1ZkQFoMPmehuxgOXgTZjrLvc-WtppMycXy3WUJAw6aYXG3Fofp38WrEXGiiKVti48xug4RzpyGG6HICwzcJVijR9mJajDOJmM_gkId_7auhfOd57Fh3Ty7c0RVtM0EcLSo6J0_h_S8RmiTXsq-ixIbutzyJwn34TgqaXAmMoALcPVHp90vEpBV3iCuUU3ERw8noV7LcURnh3TQHBLFN5VdzMYztkIjINTPgqMIZJoahtO1NPijJoAat9ifwicXIoSkCH6DKfOOj1UXhd5TPdQ9sSJ3HzyN_EcnbMf0LWkvf83cZLPFGF)

<small>[PlantUML Source](https://www.plantuml.com/plantuml/uml/jP7FIiGm4CRlVOhGexABNdlG1QjuK15Q7s1C1ZkQFoMPmehuxgOXgTZjrLvc-WtppMycXy3WUJAw6aYXG3Fofp38WrEXGiiKVti48xug4RzpyGG6HICwzcJVijR9mJajDOJmM_gkId_7auhfOd57Fh3Ty7c0RVtM0EcLSo6J0_h_S8RmiTXsq-ixIbutzyJwn34TgqaXAmMoALcPVHp90vEpBV3iCuUU3ERw8noV7LcURnh3TQHBLFN5VdzMYztkIjINTPgqMIZJoahtO1NPijJoAat9ifwicXIoSkCH6DKfOOj1UXhd5TPdQ9sSJ3HzyN_EcnbMf0LWkvf83cZLPFGF)</small>

Öffnen Sie als Basisimplementierung die Datei *[AspShowcase20230313.7z](AspShowcase20230313.7z)*.
Die Datenbank befindet sich nach Programmstart in der Datei *demo.db*.
Sie kann in DBeaver, ... betrachtet werden (SQLite Datenbank).

Implementieren Sie ein Service *TaskService*, das folgende Methoden bereitstellt.
Trennen Sie vorher das Command *TaskCmd* in *NewTaskCmd* und *EditTaskCmd*.

- **Guid AddTask(NewTaskCmd taskCmd):** Fügt einen Task zur Datenbank hinzu.
  Dabei sollen folgende Regeln beachtet werden:
    - Der Name des Tasks muss pro Team eindeutig sein.
      Prüfen Sie dies vorher in ihrer Applikationslogik.
      Werfen Sie eine entsprechende *ServiceException*, falls der Name in diesem Team schon verwendet wurde.
    - Das *ExpirationDate* muss in der Zukunft liegen.
      Wird das schon an anderer Stelle geprüft? Wenn ja, wo?

- **void EditTask(EditTaskCmd taskCmd):** Bearbeitet einen Task.
  Dabei sollen folgende Regeln beachtet werden:
    - Ist das *ExpirationDate* schon abgelaufen, darf nichts mehr bearbeitet werden.
      Dies wird mit einer entsprechenden *ServiceException* signalisiert.
    - Gibt es schon Handins, darf auch nichts mehr bearbeitet werden.
      Dies wird mit einer entsprechenden *ServiceException* signalisiert.
    - Es dürfen nur die Werte von *MaxPoints*, *Title* und *Subject* bearbeitet werden.

- **void DeleteTask(Guid guid, bool force):** Löscht einen Task.
  Ein Task darf nur gelöscht werden, wenn
    - keine Handins zugeordnet sind **oder** 
    - der Parameter force auf true gesetzt wurde.
  
  Bei einem Fehler wird eine entsprechende *ServiceException* geworfen.
  Das können Sie im Controller abfangen.

Vergessen Sie nicht, das Service in der Datei *Program.cs* mit *AddScoped* zu registrieren.
Rufen Sie nun in der Klasse *TasksController* die Servicemethoden auf.
Überlegen Sie sich, wie der Parameter *force* aus dem Delete Request übergeben werden kann (Query Parameter, Command Klasse, ...) und implementieren Sie diese Idee.

Zum Testen sind nachfolgend einige Tasks mit GUID, Title, Expiration Date, dem Team und der Anzahl der Abgaben (hand ins) aufgelistet:

```
| TaskGuid                             | TaskTitle                       | ExpirationDate      | TeamGuid                             | TeamName  | HandInCount |
| ------------------------------------ | ------------------------------- | ------------------- | ------------------------------------ | --------- | ----------- |
| 06ee74cd-61f6-96fa-9bb0-eea0698bca81 | Ut quaerat culpa ullam.         | 2023-06-25 16:00:00 | 52196e68-b907-89ac-4e7c-5838bd2ef3a1 | 4CAIF_POS | 2           |
| 084f27c2-0713-3ebd-0ba3-cb81bebc2358 | Quidem quidem ut aut.           | 2023-06-14 10:00:00 | 2465f42f-b701-322f-9366-487ef97d10bf | 5CAIF_BWM | 1           |
| 015c4f2d-480d-83a2-062a-328e3a5b865a | Fugiat nostrum nihil et.        | 2023-06-10 15:00:00 | a552e912-feb6-cd95-2985-bb5e8474bc4d | 3ACIF_BWM | 0           |
| 0923cd9a-0825-12f4-d2e4-8bfce0eb6e9a | Omnis modi sunt animi.          | 2023-04-13 09:00:00 | f5520eb5-7fa6-9033-8a91-29052d603321 | 4AAIF_POS | 0           |
| 03637b7d-bab0-e9cb-895d-223967e773a8 | Voluptatem id et quo.           | 2023-03-20 13:00:00 | 2465f42f-b701-322f-9366-487ef97d10bf | 5CAIF_BWM | 1           |
| 0541de13-cd2c-9039-fc9c-0433a22cad21 | Omnis facilis doloremque ullam. | 2023-01-25 16:00:00 | b5948d54-6ebf-9445-4eef-e2a3ee201dc6 | 5ABIF_BWM | 1           |
| 0833da89-479d-5417-3961-64f4d81f97dc | Voluptatibus ab aut quas.       | 2022-12-24 10:00:00 | b55defb5-78f5-d624-42f1-4a7a32eb6d0f | 6BCIF_POS | 1           |
| 02526890-cf8a-17b9-bc7f-fde8b45736df | Odit quasi nihil sequi.         | 2022-11-24 16:00:00 | e7c65491-ce14-3e45-61ce-43aad171ca21 | 5BKIF_POS | 2           |
| 001a6555-376c-5bfc-d862-97058ddcc6d8 | Voluptas quia impedit deleniti. | 2022-11-01 15:00:00 | 0eac6d88-f6b9-9512-574e-20db8bf09b8c | 4BBIF_DBI | 0           |
| 042618a8-746f-7f95-ccce-f94b79fa4820 | Nobis explicabo error qui.      | 2022-10-20 12:00:00 | e7c65491-ce14-3e45-61ce-43aad171ca21 | 5BKIF_POS | 0           |
```

**Verwendete Abfrage (demo.db)**

```sql
SELECT t.Guid AS TaskGuid, t.Title AS TaskTitle, t.ExpirationDate,
    te.Guid  AS TeamGuid,
    te.Name AS TeamName,
    COUNT(h.Id) AS HandInCount
FROM Teams te 
	INNER JOIN Tasks t ON (te.Id = t.TeamId) 
	LEFT JOIN Handins h ON (h.TaskId = t.Id)
GROUP BY t.Guid, t.Title, t.ExpirationDate, te.Guid, te.Name
ORDER BY t.ExpirationDate DESC;
```
