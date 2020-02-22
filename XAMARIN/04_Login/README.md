# Login mit Xamarin

## Vorbereitung der Datenbank: User Tabelle erstellen

Da wir in unserer Musterdatenbank noch keine Usertabelle haben, fügen wir sie im Rahmen einer
*Migration* hinzu. Dabei erstellen wir in der Packet Manager Console mit

```text
Add-Migration InitialCreate
```

einen "Snapshot" der Datenbank. Es wird ein Ordner *Migrations* erzeugt, der in der Datei
[20200220124112_InitialCreate.cs](TestAdministrator.Api/Migrations/20200220124112_InitialCreate.cs)
eine *Up()* und *Down()* Methode enthält. Diese Methoden würden die Datenbank von 0 weg generieren,
deswegen kommentieren wir den Inhalt dieser Methoden aus.

Mit `Update-Database` in der Packet Manager Console wird nun die Datenbank auf den generierten
Stand gebracht. Da *Up()* und *Down()* nichts machen, werden die Tabellen auch nicht geändert. Es wird
allerdings eine Tabelle *__EFMigrationsHistory* angelegt, die dem OR Mapper den Stand der Datenbank
bekannt gibt.

Danach erstellen wir die Modelklasse [User.cs](TestAdministrator.Api/Model/User.cs).
Auf Fremdschlüssel wird hier verzichtet, da sonst die bestehenden Daten (Accountnamen in *Pupil* und
*Teacher*) die referentielle Integrität verletzten würden.

Mit den folgenden Befehlen führen wir die Migration durch und erzeugen unsere Usertabelle:

```text
Add-Migration AddUserTable
Update-Database
```

Details zum Thema Migrations sind auf [Pos3xhif - 03 EFCore - 05 Migrations](https://github.com/schletz/Pos3xhif/tree/master/03%20EF%20Core/05_Migrations)
nachzulesen.

## Anpassen der UserDto Klasse

Die Datei [UserDto.cs](TestAdministrator.Dto/UserDto.cs) wird um ein enum mit der Rolle des Users
ergänzt. Die Enumeration beginnt bei 1, damit der Standardwert 0, der bei der Deserialisierung
erzeugt werden kann, nicht als "echte" Rolle identifiziert wird. Da die DTO Klassen in einer shared
library sind, können wir auch in der App bequem feststellen, welche Rolle der angemeldete User hat.

```c#
public class UserDto
{
    public enum Userrole { Pupil = 1, Teacher}
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Token { get; set; } = "";
    public Userrole Role { get; set; } = 0;
}
```

## Anpassen des Userservices und des User Controllers

In der REST API muss natürlich der Code angepasst werden, sodass die Datenbank in der Methode *GenerateToken()*
in [UserService.cs](TestAdministrator.Api/Services/UserService.cs)
abgefragt wird. Details sind im Code erklärt.

Unser User Controller in [UserController.cs](TestAdministrator.Api/Controllers/UserController.cs)
ruft nun wie gewohnt in der Methode *Login()* die Methode *GenerateToken()* auf. Außerdem wird eine
neue Route eingefügt, nämlich *api/user/create*. Diese Route erstellt einfach einen neuen User
in der Datenbank, sodass wir auch Benutzer zum Anmelden haben.

## Anpassen des Rest Services in der App

Unser RestService in [RestService.cs](TestAdministrator.App/TestAdministrator.App/Services/RestService.cs)
bekommt 2 neue Properties, die uns das Arbeiten in der App erleichtern:

```c#
public class RestService
{
    // ...
    public UserDto CurrentUser => _currentUser ?? new UserDto();
    public static RestService Instance => DependencyService.Get<RestService>();
}
```

Das erste Property liefert uns den angemeldeten User. Das ist für das Senden der Requests hilfreich.
Das zweite Property liefert uns die einzige Instanz, die automatisch über das Dependency Service
in Xamarin erstellt wird, zurück. Es ist wichtig, dass nur eine Instanz des Rest Services existiert,
da wir durch das Login den State ändern und bei einer neuen Instanz der User keinen Token mitschicken
würde.

Das Mitsenden des Tokens geschieht automatisch, denn er wird in der Methode *TryLoginAsync()* im Header
des verwendeten *HttpClient* gesetzt.

## Einbauen der Loginseite

Unsere *App* Klasse in [App.xaml.cs](TestAdministrator.App/TestAdministrator.App/App.xaml.cs)
lädt nun als erste Seite nicht mehr die Masterpage, sondern eine neu erstellte *LoginPage*.
Im Code Behind der Login Page in [LoginPage.xaml.cs](TestAdministrator.App/TestAdministrator.App/LoginPage.xaml.cs)
wird nach erfolgreichem *TryLoginAsync()* des Rest Services auf die neue Seite verwiesen.

## Clientseitiges Sperren von Features

In [ClassPage.xaml.cs](TestAdministrator.App/TestAdministrator.App/ClassPage.xaml.cs) prüfen
wir nun, ob der User ein Lehrer ist. Denn nur Lehrer dürfen die Klassendetails einsehen.

Würde diese Überprüfung nicht statt finden, so können die Daten trotzdem nicht gelesen werden,
denn der [Classes Controller](TestAdministrator.Api/Controllers/ClassesController.cs)
prüft durch die Annotation *[Authorize(Roles = "teacher")]* der *GetId()* Methode, ob der
Token einem Lehrer gehört. Allerdings würde dann ein Schüler nach dem Drücken auf die Klasse
eine HTTP Excepton (403) zu sehen bekommen, was nicht sehr schön ist.

Statt *DisplayAlert()* kann auch einfach nichts geschrieben werden, dann wird das Drücken
auf ein Listenelement einfach deaktiviert.

```c#
private async void ClassList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
{
    if (RestService.Instance.CurrentUser.Role == UserDto.Userrole.Teacher)
    {
        ClassViewModel vm = BindingContext as ClassViewModel;
        await Navigation.PushAsync(new ClassDetailPage(vm.SelectedClass.Id));
    }
    else
    {
        await App.Current.MainPage.DisplayAlert("Fehler", "Nur für Lehrer möglich.", "OK");
    }
}
```

## Testdaten

In dieser Applikation sind 2 Benutzer mit folgendem Passwort angelegt:

- *AKC11470*, Passwort 1234 (Rolle pupil)
- *schletz*, Passwort 1234 (Rolle teacher)

