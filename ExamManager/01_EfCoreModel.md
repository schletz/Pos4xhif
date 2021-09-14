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

Now we want to add some address fields (home address, parents address).

### OnModelConfiguring

#### Unique Index

#### Value Objects

More information at [docs.microsoft.com](https://docs.microsoft.com/en-us/ef/core/modeling/).

### 1 to many relationship

### 1 to 1 relationship

### Inheritance
