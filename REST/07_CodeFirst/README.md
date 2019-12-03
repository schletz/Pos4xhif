# Der Database-First-Ansatz
Den Database-First Ansatz haben wir bereits kennen gelernt. Aus einer bestehenden Datenbank werden Models und der Database-Context generiert. Natürlich kann auch aus bestehenen Models und einem Database-Context eine Datenbank generiert werden.

## Anwendung
Der Ansatz eigent sich gut für Prototyping oder kleinen Projekte die auf der "grünen Wiese" erstellt werden. Meistens wird in diesen Fällen ohne DB-Team gearbeitet. Der Entwickler kann sich so auf einfache weise, eine Datenbank erstellen lassen. Für große Projekte ist der Ansatz aber nicht zu empfehlen.

# Vorgehensweise bei Code First
Wir haben noch keine Datenbank. Sie wird aus den Models (Source Code) erstellt.

## Installation von Entity Framework Core
Wie im Database First Ansatz (den wir schon kennen gelernt haben), benötigen wir auch hier wieder einige NuGet-Packages:

+ Microsoft.EntityFrameworkCore.Tools
+ Microsoft.EntityFrameworkCore.Sqlite
+ __Microsoft.EntityFrameworkCore.Design__

Wir müssen die NuGet-Package wie folgt installieren:

```Powershell
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Sqlite
Install-Package Microsoft.EntityFrameworkCore.Design
```
Entweder direkt über die Packet Manager Console, oder etwas konfortabler über den NuGet Packet Manager.

Das Package <code>Designer</code> ermöglicht uns die Erstellung von Migrations. Diese Package muss immer im ausführbaren Projket installiert werden. Dazu später mehr.

**Unter Linux:**
```
user@hostname:~$ dotnet add package Microsoft.EntityFrameworkCore.Tools
user@hostname:~$ dotnet add package Microsoft.EntityFrameworkCore.Sqlite
user@hostname:~$ dotnet add package Microsoft.EntityFrameworkCore.Design
```

## Generieren (coden) der Model-Klassen
Als nächstes können wir die Models erstellen. Sie dienen dabei als Vorlage für die zu erstellende Datenbank, In der Applikation haben sie aber die gewohnte Funktion als Datenhalter. Es besteht also kein Unterschied.

Wir verwenden gewohnte C#-Datentypen.

z.B.:
```C#
public class Pupil
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    
    public virtual SchoolClass SchoolClass { get; set; }
    public virtual Guid SchoolClassId { get; set; }
}
```

## Generieren (coden) der Configuration
Es gibt nun 2 Möglichkeiten der Konfiguration:

### Annotations in der Model-Klasse:
In dieser Variante werden Annotations an die Properties der Models platziert:

z.B.:

```C#
[Required]
public Guid Id { get; set; }

[Required]
[MaxLength(250)]
public string FirstName { get; set; }

[Required]
[MaxLength(250)]
public string LastName { get; set; }
```
### Configuration:
In dieser Variante werden die Informationen in die Configutration der Models eingetragen.

```C#
public void Configure(EntityTypeBuilder<Pupil> builder)
{
    builder.ToTable("Pupils");
    builder.HasKey(c => c.Id);

    builder.Property(c => c.Id).IsRequired();
    builder.Property(c => c.FirstName).HasMaxLength(250).IsRequired();
    builder.Property(c => c.LastName).HasMaxLength(250).IsRequired();

    builder.HasData(
        new Pupil() 
        { 
            Id = new Guid("26a76d85-7577-4b53-abd1-4aca501a3f68"), 
            FirstName = "Vorname 1", 
            LastName = "Nachname 1",
                Gender = "M", 
                SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee")
        },
        new Pupil() 
        { 
            Id = new Guid("5699f9fe-4f2d-4c00-b226-007e0ff42ca7"), 
            FirstName = "Vorname 2", 
            LastName = "Nachname 2", 
            Gender = "M", 
            SchoolClassId = new Guid ("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee")
        },
        ...
    );
}
```

Wir müssen der Datenbank natürlich die Abhängigkeiten mitteilen, die erstellt werden sollen, also alle Relationen. Wie bereits besprochen, sind sie im Model in Form von <code>ICollection</code> abgebildet. Diese werden vom OR-Mapper in Relationen zwischen den Tabellen übertragen.

Die Abfolge der Schritte ist immer ähnlich und kann daher durchaus als _Kochrezept_ verstanden werden.

z.B.:

ein _Team_ hat viele _Messages_:

oder 

_Team_ 1 .. n _Message_

..würde so in der Team-Configuration angelegt werden:

```C#
builder.HasMany(c => c.Messages).WithOne(c => c.Team).HasForeignKey(c => c.TeamId).OnDelete(DeleteBehavior.Cascade);
```

Achtung: Natürlich muss auch im Model die relation in Form einer Collection abgebildet werden. Hierfür muss der Modifier virtual verwendet werden, da EF Core das Property überschreiben wird.

## Generieren (coden) des DBContext
In dieser Variante müssen wir den DB Context selbst programmieren. In der Database First-Variant wird er erstelt, muss aber eigentlich immer angepasst werden.

Die dafür notwendige Klasse leitet von der Klasse <code>DbContext</code> ab, im Namespace <code>Microsoft.EntityFrameworkCore</code>.

Anschließend werden die Properties gesetzt, die die Tabellen in der Datenbank repräsentieren gesetzt:

```C#
public DbSet<Pupil> Pupils { get; set; }
```

Anschließend wird der Default-LKonstruktor überschrieben:

```C#
public SchoolContext(DbContextOptions<SchoolContext> options)
    : base(options)
{ }
```

Dann überschreiben wir die Methode on <code>OnModelCreating</code> die von der Basisklasse zur Verfügung gestellt wird. Sie bindet die programmierten Configurations.

```C#
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfiguration(new PupilConfiguration());
}
```

## Migrations
Wir sind nun breit die Datenbannk zu erstellen. Wie bereits oben erwähnt hilft uns dabei der Designer. Wir erstellen nun eine Migration. Migrations syncrosisieren die Models mit der Datenbank. Wird etwas an dern Models geändert, muss eine neue Migration angelgt werden. Daraus wird ein Update der Datenbank durchgeführt.

Es gint folgende Migration-Kimmandos:

| Kommandos                         | Beschreibung                                                              |
| --------------------------------- |---------------------------------------------------------------------------|
| add-migration <migration name>    | Erstellt eine neue Migration (Migration Snapshot)                         |
| remove-migration                  | Löscht den letzten Migration Snapshot                                     |
| update-database                   | Aktualisiert die Datenbabnk basierend auf dem letzten Migration Snapshot  |
| script-migration                  | Generiert ein SQL-Script aller Migrations                                 |

## Übung
Lade die Solution <code>SPG.CodeFirstApplication</code> herunter und ergänze sie um das Model <code>SchoolClass</code>. Die Relation sieht dabei so aus, dass eine <code>SchoolClass</code> n <code>Pupils</code> hat.

#### Vorgehensweise:
+ Erstelle ein Property in <code>Pupils</code>, das eine Referenz auf <code>SchoolClass</code> hat.
+ Erstelle ein Property mit dem richtigen Datentyp in <code>Pupils</code>, das die Id der referenzierten <code>SchoolClass</code> speichert. (<code>SchoolClassId</code>)
+ Füge eine Configuration für das Model hinzu (Vergiss dabei nicht auf die Relation).
+ Ergänze den DB-Context.
+ Erstelle eine neue Migration mit einem eindeutigen Namen.
+ Führe eine Aktualsierung der Datenbank mit dem entsprechenden Kommando durch.
+ Folgende Code-Snipets kannst du für die Configuration verwenden:
+ Falls du eine neue GUID brauchst: https://www.guidgenerator.com/online-guid-generator.aspx (recht praktisch)

#### SchoolClass: builder.HasData() zum kopieren:
```C#
new SchoolClass() { Id = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee"), Name = "3AHIF", Department = "Höhere Informatik" },
new SchoolClass() { Id = new Guid("ac87ce7b-89bd-434f-a800-b2979d745c1b"), Name = "3BHIF", Department = "Höhere Informatik" },
new SchoolClass() { Id = new Guid("1712daf8-bf01-4f88-905b-74ec9498d077"), Name = "4AHIF", Department = "Höhere Informatik" }
```

#### Pupil: builder.HasData() zum kopieren:
```C#
new Pupil() { Id = new Guid("26a76d85-7577-4b53-abd1-4aca501a3f68"), FirstName = "Vorname 1", LastName = "Nachname 1", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
new Pupil() { Id = new Guid("5699f9fe-4f2d-4c00-b226-007e0ff42ca7"), FirstName = "Vorname 2", LastName = "Nachname 2", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
new Pupil() { Id = new Guid("3404ce31-e751-44cb-b84a-3b318a017176"), FirstName = "Vorname 3", LastName = "Nachname 3", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
new Pupil() { Id = new Guid("1b71952b-4695-4741-92a0-b2fb9dfa6851"), FirstName = "Vorname 4", LastName = "Nachname 4", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
new Pupil() { Id = new Guid("7b66c7c6-2898-456e-95bf-37af3f97e799"), FirstName = "Vorname 5", LastName = "Nachname 5", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
new Pupil() { Id = new Guid("cf3bd38f-3e4f-4202-a6ee-102e00c03a2a"), FirstName = "Vorname 6", LastName = "Nachname 6", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
new Pupil() { Id = new Guid("65549b37-0de2-4549-9a0c-90311bad52f1"), FirstName = "Vorname 7", LastName = "Nachname 7", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
new Pupil() { Id = new Guid("d69d01ba-afed-40ea-b54e-63c0fbd25abd"), FirstName = "Vorname 8", LastName = "Nachname 8", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
new Pupil() { Id = new Guid("a16970d2-2a84-4a76-8991-0ecec1eeb1c8"), FirstName = "Vorname 9", LastName = "Nachname 9", Gender = "M", SchoolClassId = new Guid("ac87ce7b-89bd-434f-a800-b2979d745c1b") },
new Pupil() { Id = new Guid("38cf31d8-a3a9-4e4d-86d9-aacb52c52c1b"), FirstName = "Vorname 10", LastName = "Nachname 10", Gender = "M", SchoolClassId = new Guid("ac87ce7b-89bd-434f-a800-b2979d745c1b") },
new Pupil() { Id = new Guid("ef2e0d3d-91b8-44c1-b335-183a82f517b2"), FirstName = "Vorname 11", LastName = "Nachname 11", Gender = "M", SchoolClassId = new Guid("ac87ce7b-89bd-434f-a800-b2979d745c1b") },
new Pupil() { Id = new Guid("ad4ab73a-8cba-4ad6-9079-3f546c0f8589"), FirstName = "Vorname 12", LastName = "Nachname 12", Gender = "M", SchoolClassId = new Guid("1712daf8-bf01-4f88-905b-74ec9498d077") },
new Pupil() { Id = new Guid("fcd40d7b-b676-43a9-bec5-1d2f9301a450"), FirstName = "Vorname 13", LastName = "Nachname 13", Gender = "M", SchoolClassId = new Guid("1712daf8-bf01-4f88-905b-74ec9498d077") },
new Pupil() { Id = new Guid("7893a991-cb8c-457b-84b6-87329f70d9b6"), FirstName = "Vorname 14", LastName = "Nachname 14", Gender = "M", SchoolClassId = new Guid("1712daf8-bf01-4f88-905b-74ec9498d077") },
new Pupil() { Id = new Guid("65dc791d-8109-4f63-9bf2-fab745af866d"), FirstName = "Vorname 15", LastName = "Nachname 15", Gender = "M", SchoolClassId = new Guid("1712daf8-bf01-4f88-905b-74ec9498d077") }
```
+ achte auf die GUID's bei den Objekt-Referenzen!!

##### (Für die schnellen)
+ Füge in der <code>PupilConfiguration</code>  weitere Datensätze hinzu und füge eine weitere Migration hinz. (2 Migrations in der Solution)
+ Erstelle CRUD-Methoden in der <code>Program.cs</code>. Du kannst dich dabei an die Controller-Logik der Übung REST -  Entity Framework halten.
