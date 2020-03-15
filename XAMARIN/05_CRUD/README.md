# CRUD Operationen in Xamarin Forms

Die Solution [TestAdministrator.sln](TestAdministrator.sln) beinhaltet den neuesten Stand unserer
Testverwaltungs-App. Der Quelltext ist kommentiert und beschreibt, warum Lösungen so implementiert
wurden.

Bei Buildproblemen starte bei geschlossenem Visual Studio die Datei
[cleanSolution.cmd](cleanSolution.cmd). Sie löscht alle Builds und Visual Studio Einstellungen.
Danach dauert es beim Laden der Solution etwas, bis die Pakete wieder nachgeladen wurden (sichbar
durch Klick auf die Benachrichtigung links unten). Danach schließe Visual Studio und nach erneutem
Öffnen lässt sich die Applikation erstellen.

In der Datenbank gibt es 2 bestehende Benutzer: *schletz* (als Lehrer) und *AKC11470* (als Schüler)
mit dem Kennwort *1234*.

## Der Test Controller

Wir erinnern uns aus dem Kapitel REST, dass den Datenbankoperationen bestimmte HTTP Requestarten
zugeordnet sind:

| Operation | Controller Action             | Route im Test Controller      |
| --------- | ----------------------------- | ----------------------------- |
| Create    | POST Request im Controller.   | POST /api/test                |
| Read      | GET Request im Controller.    | GET /api/testsbyuser/{userId} |
| Update    | PUT Request Im Controller.    | PUT /api/test/(testId)        |
| Delete    | DELETE Request im Controller. | DELETE /api/test/(testID)     |

Diese Requestarten müssen im Test Controller realisiert werden, der in
[TestController.cs](TestAdministrator.Api/Controllers/TestController.cs) implementiert ist.
Der alte Dashboard Controller wurde entfernt, statt dessen liefert die Route
*/api/testsbyuser/(userId)* mittels HttpGet Annotation die Daten:

```c#
[HttpGet("/api/testsbyuser/{userId}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public ActionResult<IEnumerable<TestDto>> Get(string userId)
{
    ...
}
```

Außerdem wird noch die Route */api/lessons/(teacherId)* bereitgestellt, die eine Collection aller
Unterrichtsstunden des Lehrers zurückgibt. Dies ist für das Frontend wichtig, denn der Lehrer soll
nur Tests für seine Fächer und Klassen, in denen er auch unterrichtet, eintragen können.

Wichtig ist das Setzen von *[Authorize]* über der Klassendefinition, damit nur angemeldete Benutzer
Daten vom Controller bekommen.

### Achtung vor Sicherheitslücken: Prüfen des Users

Im [TestController](TestAdministrator.Api/Controllers/TestController.cs) wird bei manchen Routen
geprüft, ob ein Lehrer auch nur die eigenen Tests ändert oder bearbeitet:

```c#
public ActionResult<TestDto> Post([FromBody] TestDto testinfo)
{
    if (testinfo.Teacher != _authTeacherId) { return Forbid(); }
    ...
}
```

Da über die *Authorize* Annotation nur Konstanten angegeben werden können, würde die fixe
Angabe einer User-ID nichts nützen. Deswegen müssen wir in der Route prüfen, ob der User auch
die Aktion auslösen darf.

### Die DTO Klassen

Die Klasse [UserDto](TestAdministrator.Dto/UserDto.cs) wurde dahingehend angepasst, dass sie nun
die Properties *TeacherId* bzw. *PupilId* beinhaltet. Sie stellen die in der Datenbank verwendeten
ID Werte für Lehrer (also das Lehrerkürzel wie SZ) bzw. für Schüler (die Schülernummer) dar. Der
[Usercontroller](TestAdministrator.Api/Controllers/UserController.cs) ermittelt diese Werte
und gibt sie der Applikation mit. Das ist notwendig, damit die App entsprechende Requests an den
Testcontroller leichter senden kann.

Der Testcontroller selbst verwendet die Klasse [TestDto](TestAdministrator.Dto/TestDto.cs). Hier
werden die Informationen für die Seiten
[DashboardPage](TestAdministrator.App/TestAdministrator.App/DashboardPage.xaml) und
[EditTestPage](TestAdministrator.App/TestAdministrator.App/EditTestPage.xaml) in der App
übermittelt.

### Test mit Postman

Führe die REST API aus und sende folgende Requests in Postman:

- POST https://localhost:8080/api/user/login mit den Daten *{"username": "schletz", "password": "1234"}*.
  Es kommt ein JSON Objekt (*UserDto*) mit gesetztem Token zurück. Kopiere den gesamten Token in die
  Zwischenablage.
- Sende nun GET https://localhost:8080/api/lessons/SZ ohne Authorizsation. Es wird mit HTTP 401
  geantwortet. Nun kopiere den Token in der Option *Authorizsation - Type - Bearer Token*. Der
  Server antwortet mit den Klassen und Fächern laut Stundenplan.
- Sende GET https://localhost:8080/api/testsbyuser/schletz mit Authorizsation. Es werden alle
  eingetragenen Tests geliefert.
- Sende GET https://localhost:8080/api/testsbyuser/AKC11470 mit Authorizsation. Es wird HTTP 403
  geliefert, da der User schletz nicht die Tests eines anderen Users (AKC11470) einsehen darf.

## Das Test Repository

In der App befindet sich im Ordner Services die Klasse
[TestRepository](TestAdministrator.App/TestAdministrator.App/Services/TestRepository.cs).

### Wozu ein Repository

Ein Repository fasst alle Zugriffe auf die Datenbank zusammen. In der App bedeutet Zugriff auf
die Datenbank das Senden von GET, POST, PUT und DELETE Requests über das RestService. Die
Vorteile der Bündelung dieser Requests sind:

- Nach außen wird eine *ObservableCollection* mit den Daten (den Tests) angeboten. Das Viewmodel kann
  auf diese Collection zugreifen und die View wird automatisch aktualisiert.
- Da das ViewModel das Repository fertig initialisiert übergeben bekommt benötigt es keine asynchrone
  Factorymethode mehr.
- Es ist zu Testzwecken austauschbar, somit kann die App auch mit Musterdaten getestet werden.

### Der Konstruktor und *CreateAsync*: Umgang mit Dependencies

Unser Repository bietet eine Methode *CreateAsync()* an. Diese bekommt 2 Parameter: den angemeldeten
User und das Rest Service. Natürlich können wir auch im Repository mit *RestService.Instance* zugreifen,
das hat jedoch Nachteile:

- Dependencies (also Abhängigkeiten mit anderen Programmklassen) sollten klar ausgewiesen werden.
  Das geht am Besten im den Konstruktor oder der Create Methode, denn so muss der Anwender schon
  entsprechende Instanzen mitgeben.
- Bei Tests können Mockup Klassen übergeben werden.
- Der Code ist austauschbarer.

Konkret sieht die Implementierung so aus:

```c#
public static async Task<TestRepository> CreateAsync(UserDto user, RestService restService)
{
    var tests = await RestService.Instance.SendAsync<ObservableCollection<TestDto>>(
        HttpMethod.Get, "testsbyuser", user.Username);

    if (!string.IsNullOrEmpty(user.TeacherId))
    {
        var lessons = await RestService.Instance.SendAsync<List<LessonDto>>(
            HttpMethod.Get, "lessons", user.TeacherId);
        return new TestRepository(restService, user, tests, lessons);
    }
    return new TestRepository(restService, user, tests);
}

private TestRepository(RestService restService, UserDto user, ObservableCollection<TestDto> tests, List<LessonDto> lessons)
{
    _restService = restService;
    _user = user;
    Tests = tests;
    Lessons = lessons;
}
```

### Immutable Properties

Ein weiterer Vorteil dieser Methode ist folgender: Alle Properties sind read-only. Das bedeutet, dass
sich der Programmierer darauf verlassen kann, dass sie nicht von außen auf eventuell problematische
Werte (null) gesetzt wurden und im Konstruktor sicher initialisiert wurden. Offene setter sollten
generell vermieden werden und sind nur in Entity- oder DTO Klassen flächig vorhanden.

## Anpassungen im ViewModel

Das ViewModel der Dashboardpage in der Klasse
[DashboardViewModel](TestAdministrator.App/TestAdministrator.App/ViewModels/DashboardViewModel.cs)
hat nun (wieder) einen public Konstruktor. Er bekommt ein fertig initialisiertes Test Repository und
den Navigation Stack.

Das Property *TestInfos*, an das sich die ListView bindet, ist nun die Observable Collection im
Repository. So wird - wenn im Repository etwas hinzugefügt oder gelöscht wird - die View automatisch
aktualisiert.

### Commands

Wie unter WPF gibt es auch hier die Möglichkeit, das Buttons statt eines Eventhandlers auf ein
Command Property zugreifen. Xamarin stellt mit der Klasse *Command&lt;T&gt;* eine generische Klasse
zur Verfügung. Es braucht 3 Dinge, damit ein Button eine Methode im ViewModel aufrufen kann:

- Ein (public) Property vom Typ *ICommand*. Dies ist read-only.
- Eine Methode, die dem Property zugewiesen wird. Dies wird im Konstruktor des ViewModels gemacht.
- In XAML wird mit dem Attribut *Command* auf das Property verwiesen.

Der Vorteil der Command Properties ist der Zugriff auf alle Felder des ViewModels, ohne dass alles
public gemacht werden muss.

### Navigations

Beim Klicken auf den *New* Button im Menü soll eine neue Seite
([EditTestPage](TestAdministrator.App/TestAdministrator.App/EditTestPage.xaml)) angezeigt werden.
Damit das gelingt, wird in der
[LoginPage](TestAdministrator.App/TestAdministrator.App/LoginPage.xaml.cs)
nach erfolgreichem Login zuerst eine neue Navigation Page erzeugt. Sie beinhaltet einen Navigation
Stack. Danach wird das Test Repository mit der *CreateAsync()* Methode erzeugt. Am Ende wird dann
die neue Dashboard Page mit dem Viewmodel auf den Navigation Stack gelegt:

```c#
NavigationPage newNavigation = new NavigationPage();
TestRepository testRepository = await TestRepository.CreateAsync(RestService.Instance.CurrentUser, RestService.Instance);
await newNavigation.PushAsync(new DashboardPage(new DashboardViewModel(testRepository, newNavigation.Navigation)));
```

Da das ViewModel nun auf die Navigation zugreifen kann, können Commandmethoden nun neue Seiten
aufrufen und auch zurückgehen.

## Die Pages DashboardPage und EditTestPage

In den Dateien [DashboardPage.xaml](TestAdministrator.App/TestAdministrator.App/DashboardPage.xaml) und
[EditTestPage.xaml](TestAdministrator.App/TestAdministrator.App/EditTestPage.xaml) wurden folgende
Features eingebaut:

### Das Menü

Die Dashboard Page hat nun ein Menü mit 3 Buttons. Hier ist es möglich, neue Tests anzulegen, Tests
zu bearbeiten oder Tests zu löschen. Damit ein Menü angelegt werden kann, gibt es in XAML das Element
*ToolbarItem*:

```xml
<ContentPage.ToolbarItems>
    <ToolbarItem Text="Add"
        IconImageSource="{local:ImageResource TestAdministrator.App.Resources.add.png}"
        Order="Primary"  
        Command="{Binding NewItem}"
        Priority="0" />
    <ToolbarItem Text="Edit" ... />
    <ToolbarItem Text="Delete" ... />
</ContentPage.ToolbarItems>
```

Jeder Menüpunkt hat einen Text, ein Bild, ein dazugehöriges Command im ViewModel und eine Position
(*Priority*). Die Eigenschaft *IconImageSource* wird später genauer beschrieben, sie lädt über eine
XAML Extension ein eingebettetes Bild.

### Kontextmenü: Buttons in einer Liste

Oft soll für jeden Datensatz einer Liste ein Button oder ein Kontextmenü angeboten werden. Die
Schwierigkeit ist dabei, den aktuellen Datensatz im entsprechenden Command zu ermitteln. Deswegen
brauchen wir neben dem Command die Eigenschaft *CommandParameter*.

Zuerst geben wir unserer ContentPage mit *x:Name="MyDashboardPage"* einen Namen. Da die Bindings
in Listenelementen auf den aktuellen Datensatz verweisen, brauchen wir diesen Namen. Durch
*Source={x:Reference MyDashboardPage}, Path=BindingContext.DeleteItem}* haben wir nämlich
Zugriff auf unser ViewModel und können das Command mit dem Propertynamen *DeleteItem* aufrufen.

Der Command Parameter ist notwendig, damit die Methode *DeleteItem()* im Viewmodel den zu löschenden
Datensatz übergeben bekommt. Die genaue Implementierung ist im
[ViewModel der Dashboard Page](TestAdministrator.App/TestAdministrator.App/ViewModels/DashboardViewModel.cs)
zu sehen.

Ein Button in der Liste kann dann so in XAML realisiert werden:

```xml
<Button Text="Löschen"
        Command="{Binding Source={x:Reference MyDashboardPage}, Path=BindingContext.DeleteItem}" 
        CommandParameter="{Binding .}"></Button>
```

Ein Eintrag im Kontextmenü sieht dann fast ident aus:

```xml
<ViewCell.ContextActions>
<MenuItem Text="Delete"
          Command="{Binding Source={x:Reference MyDashboardPage}, Path=BindingContext.DeleteItem}"
          CommandParameter="{Binding .}"/>
</ViewCell.ContextActions>
```

### Eingebettete Bilder

Unser Menü verwendet Bilder, die eingebettet wurden. Mit normalen XAML Mitteln kann leider nicht
darauf zugegriffen werden, deswegen wurde von
[Microsoft Docs](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/images?tabs=windows#xaml)
die Klasse [ImageResourceExtension](TestAdministrator.App/TestAdministrator.App/Resources/ImageResourceExtension.cs)
in den Ordner Resources kopiert. Achte auf den Namespace, er ist ident mit den der einzelnen Pages.

Nun verweisen wir in der Pagedefinition in XAML auf diesen Namespace und nennen ihn *local*.
Wichtig ist auch, dass die Seite eine Klassendefinition (*x:Class*) besitzt, die die verwendete
Klasse im Code Behind angibt.

```xml
<ContentPage ...
             xmlns:local="clr-namespace:TestAdministrator.App"
             x:Class="TestAdministrator.App.DashboardPage">
```

Nun können im Ordner *Resources* Bilder eingefügt werden. In Visual Studio muss dann bei jedem Bild
im Solution Explorer unter Properties *Build Action: Embedded resource* gesetzt werden.

Beim Zugriff auf die Datei muss der Pfad mit einem Punkt umgesetzt werden. Die Datei wird nämlich
über Assembly.Pfad.Name angesprochen. Konkret sieht das im unserem Fall so aus:

```xml
<ToolbarItem IconImageSource="{local:ImageResource TestAdministrator.App.Resources.add.png}" ... />
```

Auf https://www.flaticon.com/ gibt es viele fertige Symbole zum Download.

## Upload als Azure AppService

Am Ende kann das Api Projekt (natürlich nicht die App) in Azure als App Service laufen. Wird
SQLite verwendet, so empfiehlt sich folgende Vorgehensweise:

- Durchführen des Publishings wie auf https://github.com/schletz/Pos4xhif/blob/master/Azure/02_AppServiceWebAPI.md#publishing beschrieben.
- Aktivieren des Development Profiles (https://github.com/schletz/Pos4xhif/blob/master/Azure/02_AppServiceWebAPI.md#aktivieren-des-development-profiles)
- Herstellen der FTP Verbindung (https://github.com/schletz/Pos4xhif/blob/master/Azure/02_AppServiceWebAPI.md#ftp-zugriff-auf-das-app-service)
- Kopieren der SQLite Datenbank in das App Service über einen FTP Client wie WinSCP.

Am Ende muss in der Datei [appsettings.json](TestAdministrator.App/TestAdministrator.App/appsettings.json)
der App noch der Azure Server eingetragen werden. Da das RestService den Pfad *api* nicht mitsendet,
muss dieser hier als Pfad angehängt werden:

```javascript
{
  "ServiceUrl": "https://mydomain.azurewebsites.net/api"
}
```

### Verwenden von SQL Server

Natürlich kann auch eine SQL Server Datenbank verwendet werden. Eine Anleitung steht auf
https://github.com/schletz/Pos4xhif/blob/master/Azure/01_Database.md zur Verfügung.

In der Api muss dann der Verbindungsstring in der Datei [appsettings.json](TestAdministrator.Api/appsettings.json)
angepasst werden. Dabei wird der Connection String aus dem Azure Portal *Settings - Connection Strings*
in der SQL Server Datenbank übertragen:

```javascript
{
  "AppSettings": {
    "Database": "Server=tcp:AAAA,1433;Initial Catalog=bbbb;Persist Security Info=False;User ID=cccc;Password=dddd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;",
    ...
  },
  ...
}
```

> **Achtung:** Die Datei *appsettings.json* darf natürlich nicht in ein öffentliches Repository
> geladen werden. Sie ist mit einer *.gitignore* Datei vom Upload auszuschließen.

In *ConfigureServices()* der Datei [Startup.cs](TestAdministrator.Api/Startup.cs) wird nun
statt *UseSqlite()* die Methode *UseSqlserver()* verwendet:

```c#
services.AddDbContext<TestsContext>(options =>
    options.UseSqlServer(Configuration["AppSettings:Database"])
);
```
