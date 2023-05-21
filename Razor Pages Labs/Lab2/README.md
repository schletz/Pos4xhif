# Lab 2 zu ASP.NET Core: CRUD Operationen in Controllern

![](https://www.plantuml.com/plantuml/svg/jP7FIiGm4CRlVOhGexABNdlG1QjuK15Q7s1C1ZkQFoMPmehuxgOXgTZjrLvc-WtppMycXy3WUJAw6aYXG3Fofp38WrEXGiiKVti48xug4RzpyGG6HICwzcJVijR9mJajDOJmM_gkId_7auhfOd57Fh3Ty7c0RVtM0EcLSo6J0_h_S8RmiTXsq-ixIbutzyJwn34TgqaXAmMoALcPVHp90vEpBV3iCuUU3ERw8noV7LcURnh3TQHBLFN5VdzMYztkIjINTPgqMIZJoahtO1NPijJoAat9ifwicXIoSkCH6DKfOOj1UXhd5TPdQ9sSJ3HzyN_EcnbMf0LWkvf83cZLPFGF)

<small>[PlantUML Source](https://www.plantuml.com/plantuml/uml/jP7FIiGm4CRlVOhGexABNdlG1QjuK15Q7s1C1ZkQFoMPmehuxgOXgTZjrLvc-WtppMycXy3WUJAw6aYXG3Fofp38WrEXGiiKVti48xug4RzpyGG6HICwzcJVijR9mJajDOJmM_gkId_7auhfOd57Fh3Ty7c0RVtM0EcLSo6J0_h_S8RmiTXsq-ixIbutzyJwn34TgqaXAmMoALcPVHp90vEpBV3iCuUU3ERw8noV7LcURnh3TQHBLFN5VdzMYztkIjINTPgqMIZJoahtO1NPijJoAat9ifwicXIoSkCH6DKfOOj1UXhd5TPdQ9sSJ3HzyN_EcnbMf0LWkvf83cZLPFGF)</small>

Öffnen Sie als Basisimplementierung die Datei *[AspShowcase20230307.7z](AspShowcase20230307.7z)*.
Die Datenbank befindet sich nach Programmstart in der Datei *demo.db*.
Sie kann in DBeaver, ... betrachtet werden (SQLite Datenbank).
Implementieren Sie einen Controller *HandinsController*, der folgende Routen bereitstellt:

### GET /api/handins

Diese Route liefert alle Abgaben (hand-ins) als JSON Array.
Es sollen folgende Daten geliefert werden:

```json
{
    "guid": "string",
    "created": "string",
    "description": "string",
    "documentUrl": "string",
    "studentGuid": "string",
    "studentFirstname": "string",
    "studentLastname": "string",
    "studentEmail": "string",
    "taskGuid": "string",
    "taskTitle": "string",
    "taskTeacherFirstname": "string",
    "taskTeacherLastname": "string",
    "taskTeacherEmail": "string"
}
```

### GET /api/handins/(guid)

Liefert die Daten einer bestimmten Abgabe.
Sie wird durch die GUID angegeben.
Liefert den HTTP Status 404 (not found), wenn die Abgabe nicht gefunden wurde.
Wenn die Angabe gefunden wurde, liefert die Route ein JSON Object mit den oben angeführten Daten.

### GET /api/handins?studentGuid=(guid)

Die Route liefert alle Abgaben, die von der angegebenen Student GUID stammen.
Verwenden Sie *FromQuery*, um auf den Query Parameter *studentGuid* zugreifen zu können.
Liefert ein JSON Array mit den Abgaben, die Daten sind oben beschrieben.

### POST /api/handins

- Legen Sie eine Klasse `NewHandinCmd` an.
  Sie beinhaltet alle Felder, die zur Erstellung eines hand-in Datensatzes notwendig sind:
  *StudentGuid, TaskGuid, Description, DocumentUrl*.
  Das Feld *Created* wird im Controller erzeugt, sonst könnte es ja von einem Studierenden rückdatiert werden.
- Vergessen Sie nicht, das Mapping für Automapper zu registrieren.
- Validieren Sie die Felder *Description* und *DocumentUrl* so, dass sie eine Länge zwischen 1 und 255 Zeichen haben.
- Die Fremdschlüsselwerte (*StudentGuid* und *TaskGuid*) müssen natürlich korrekt sein.
- Fügen Sie im *HandinController* die entsprechende Route hinzu, sodass das hand-in erstellt wird.
- Ist das *ExpirationDate* des entsprechenden Tasks bereits überschritten (*DateTime.Now > ExpirationDate*), so wird HTTP 403 (forbidden) zurückgegeben.
  Das kann mit `return Forbid();` erreicht werden.
- Beachten Sie, dass die Felder *StudentId* und *TaskId* als UNIQUE definiert sind.
  Es darf also nur eine Abgabe pro Student und Task erfolgen.
  Prüfen Sie daher Fehlermeldungen der Datenbank *(DbUpdateException)*.
  Wenn Sie also das selbe hand-in mehrmals anlegen wollen, sollten Sie beim 2. Request einen Fehler bekommen.
- Im Erfolgsfall liefert der Controller HTTP 201 (created) mit der GUID des neuen hand-ins.
- Im Fehlerfall liefert der Controller HTTP 400 (bad request) mit einer Fehlerbeschreibung.
- Testen Sie die Route in Postman (raw body, type: JSON).

### PUT /api/handins/(guid)

- Legen Sie eine Klasse `EditHandinCmd` an.
  Sie beinhaltet nur die Felder, die editiert werden dürfen (*Description, DocumentUrl*) sowie die GUID.
- Vergessen Sie nicht, das Mapping für Automapper zu registrieren.
- Validieren Sie die Felder *Description* und *DocumentUrl* so, dass sie eine Länge zwischen 1 und 255 Zeichen haben.
- Fügen Sie im *HandinController* die entsprechende Route hinzu, sodass ein hand-in geändert werden kann.
- Wurde die GUID des hand-ins nicht gefunden, wird HTTP 404 (not found) zurückgegeben.
- Ist das *ExpirationDate* des entsprechenden Tasks bereits überschritten (*DateTime.Now > ExpirationDate*), so wird HTTP 403 (forbidden) zurückgegeben.
- Testen Sie die Route in Postman (raw body, type: JSON).

### DELETE /api/handins/(guid)

- Fügen Sie im *HandinController* die entsprechende Route hinzu, sodass ein hand-in gelöscht werden kann.
- Ist das *ExpirationDate* des entsprechenden Tasks bereits überschritten (*DateTime.Now > ExpirationDate*), so wird HTTP 403 (forbidden) zurückgegeben.
- Wurde die GUID nicht gefunden, wird HTTP 404 (not found) zurückgegeben.
- Im Erfolgsfall wird HTTP 204 (no content) zurückgegeben.
- Testen Sie die Route in Postman.

