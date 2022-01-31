# Razor Pages Intro

## Anlegen des Projektes

```
rd /S /Q StoreManager
md StoreManager
cd StoreManager
md StoreManager.Application
cd StoreManager.Application
dotnet new classlib
dotnet add package Microsoft.EntityFrameworkCore --version 6.*
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 6.*
dotnet add package Microsoft.EntityFrameworkCore.Proxies --version 6.*
dotnet add package Bogus --version 34.*
cd ..
md StoreManager.Test
cd StoreManager.Test
dotnet new xunit
dotnet add reference ..\StoreManager.Application
cd ..
md StoreManager.Webapp
cd StoreManager.Webapp
dotnet new webapp
dotnet add reference ..\StoreManager.Application
cd ..
dotnet new sln
dotnet sln add StoreManager.Webapp
dotnet sln add StoreManager.Application
dotnet sln add StoreManager.Test
start StoreManager.sln

```