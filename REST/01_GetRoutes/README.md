# GET Routen mit ASP.NET Core

Um ein einfaches Webservice mit ASP.NET Core zu erstellen, wird in Visual Studio 2019 mittels *Create a
new project* die Vorlage *ASP.NET Core Web Application* gewählt. Nach der Vergabe eines Namens wird
in den Projekteinstellungen der Punkt *API* gewählt. Die Checkbox bei *Configure für HTTPS* kann
abgehakt werden:

![](create_api_project.png)

Nach dem Erstellen des Projektes können die Controller angelegt werden. Im Musterprojekt ist ein
Controller mit dem Namen *PupilController* angelegt. Der Name ist für das Routing wichtig, denn
der Request */api/pupil* wird automatisch an den Controller mit dem Namen *PupilController* 
weitergegeben.

Sollen weitere Controller angelegt werden, so kann dies in Visual Studio unter *Controllers* - 
*Add* - *Controller* erledigt werden:

![](add_controller.png)

## Der Pupil Controller und seine Funktionen
Wird ein leerer Controller angelegt, so werden 2 Annotations über die Klassendefinition 
geschrieben:
```c#
[Route("api/[controller]")]
[ApiController]
public class PupilController : ControllerBase
{
}
```

Die Annotation *[Route("api/[controller]")]* bewirkt, dass Requests mit der URL 
*/api/controllername* hier bearbeitet werden. Natürlich könnte man statt *[controller]*
auch den Namen des Controllers (also *Pupil*) schreiben, so ist es jedoch allgemeiner.

### GET Routen
Damit ein Controller auf GET Anfragen reagiert, wird eine Methode mit der Annotation 
*HttpGet* versehen. Der Methodenname kann dabei beliebig gewählt werden.
```c#
[HttpGet]
public IActionResult Get()
{
    return Ok(db.Schueler.Select(s => new { s.Vorname, s.Zuname }));
}
```

Die Methode *Ok* bewirkt, dass das Ergebnis der Abfrage als JSON mit dem Statuscode 200 
zurückgegeben wird.

Da nichts weiter angegeben wird, wird diese Methode bei */api/pupil* aufgerufen. Möchten wir nun 
einzelne Schüler abfragen, brauchen wir eine dynamischere Definition. Wenn bei */api/pupil/1001* der
Schüler mit der ID 1001 zurückgegeben werden soll, so können wir mit folgender Annotation den 
letzten Parameter in die Variable id schreiben:
```c#
[HttpGet("{id}")]
public IActionResult GetById(string id)
{
}
```

Beachte: Parameter sind immer Strings. Er kann zwar automatisch geparst
werden, indem die Methode einen Parameter vom Typ *int* bekommt, eine Fehlerbehandlung ist 
dann allerdings nicht möglich.

Möchten wir auf einen Query String der Form */api/pupil/byId?id=1001* reagieren, so ist das 
ebenfalls möglich:
```c#
[HttpGet("byId")]
public IActionResult GetWithQuerystring([FromQuery]string id)
{
}
```

Die Routen waren bis jetzt immer relativ, d. h. sie werden zur Route des Controllers "dazugegeben".
Möchten wir eine Route definieren, die auf */api/count* reagiert, so können wir durch einen 
Schrägstich am Anfang eine absolute Route definieren:
```c#
[HttpGet("/api/count")]
public IActionResult GetPupilCount()
{
}
```
