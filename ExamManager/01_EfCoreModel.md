# Example project: Exam manager

![](er_diagram.png)

## Creation of your solution

```text
rd /S /Q ExamManager
md ExamManager
cd ExamManager
md ExamManager.App
cd ExamManager.App
dotnet new classlib
dotnet add package Microsoft.EntityFrameworkCore --version 5.0.9
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 5.0.9
dotnet add package Bogus --version 33.1.1
cd ..
md ExamManager.Test
cd ExamManager.Test
dotnet new xunit
dotnet add reference ..\ExamManager.App
cd ..
dotnet new sln
dotnet sln add ExamManager.App
dotnet sln add ExamManager.Test
start ExamManager.sln
```

## New EF Core features in EF Core 5

### Nullable reference types (C# 8)
To enable nullable reference types, edit your ExamManager.App.csproj file
and set the *nullable* option:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net5.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <!-- ... -->

</Project>
```

### Private constructors and private setters

To ensure the initialization of all required properties, you can define a
(public) constructor with all your required fields (*@RequiredArgsConstructor,* in Lombok).
EF Core doesn't support changing the primary key, therefore the property *Id*
has a private setter. EF Core requires a parameterless constructor, so we can
use a private constructor for this.

Because *Account*, *Lastname*, ... is initialized by EF Core,
C# 8 won't recognize this. So we have to initialize these properties with the
*null forgiving* operator (!).

*Account*, *Lastname*, *Firstname* are required, EF Core will create NOT NULL
fields in our database. *Email* is optional, therefore we define this property
with *string?*. 

```c#
public class Student
{
    // Constructor without id (Id is database generated)
    public Student(string account, string lastname, string firstname)
    {
        Account = account;
        Lastname = lastname;
        Firstname = firstname;
    }
    private Student() { }
    // Id -> primary key, int Id -> autoincrement
    public int Id { get; private set; }
    public string Account { get; set; } = default!;
    public string Lastname { get; set; } = default!;
    public string Firstname { get; set; } = default!;
    private string? Email { get; set; }
}
```

### Value Objects and Records

Now we want to add some address fields (home address, parents address). Our Address class 
- should implement *Equals()* and
- it shout be immutable.

C# 9 introduced records. A *positional record* provides immutability and implements equality for all properties.
So we use a positional record to create our address class:
```c#
  public record Address(
      [property: MaxLength(255)] string City,
      [property: MaxLength(255)] string Zip,
      [property: MaxLength(255)] string Street
      )
  {
      public string FullAddress => $"{Zip} {City}, {Street}";
  }
```

Now we can use this type in our *Student* class:

```c#
  public class Student
  {
      // ...
      public Address Home { get; set; } = default!;
      public Address? Parents { get; set; }
  }
```
More information on value objects can be found at [docs.microsoft.com](https://docs.microsoft.com/en-us/ef/core/modeling/).

#### OnModelConfiguring

Without further configuration EF Core will try to create a table *Address*. But we want to include the address information
as columns in our student table. With *OwnsOne()* we can configure our properties
*Home* and *Parents* as a value object:

```c#
public class ExamContext : DbContext
{
    public DbSet<Student> Students => Set<Student>();
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Exam.db");
        optionsBuilder.LogTo(
            Console.WriteLine,
            Microsoft.Extensions.Logging.LogLevel.Information);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>().OwnsOne(s => s.Home);
        modelBuilder.Entity<Student>().OwnsOne(s => s.Parents);
    }
}
```

### Creation of our database

We use a unit test to create our SQLite file database. The
test method *EnsureDatabaseCreatedTest()* calls
*EnsureDeleted()* to delete the old file database. 
*EnsureCreated()* will create a new SQLite Database based on
our model (code first approach).

```c#
public class ExamContextTests
{
    [Fact]
    public void EnsureDatabaseCreatedTest()
    {
        using var context = new ExamContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
```

### Unique Index

### 1 to many relationship

### 1 to 1 relationship

### Inheritance
