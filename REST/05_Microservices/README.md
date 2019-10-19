# Microservices mit Microsoft .NET

WebApi Anwendungen sind auch die Basis von Microservices. Große Anwendungen werden dabei in mehrere kleinere, 
genau abgegrenzte REST APIs unterteilt.

![](microservice_architecture.png)

<sup>Quelle: https://docs.microsoft.com/en-us/dotnet/architecture/microservices/secure-net-microservices-web-applications/</sup>

Zum Erstellen wird heute oft Docker verwendet. Docker für Windows kann von 
[download.docker.com](https://download.docker.com/win/stable/Docker%20for%20Windows%20Installer.exe)
über diesen Link auch ohne Login geladen werden. In Visual Studio Code kann mit der Extension *Docker*
das Erstellen von Dockerfiles vereinfacht werden. Außerdem wird dann in der Iconleiste links die
Möglichkeit angeboten, die verfügbaren Dockerimages einzusehen.

Unter https://dotnet.microsoft.com/learn/aspnet/microservice-tutorial/intro findet sich eine Schritt für Schritt
Anleitung zum Erstellen von Microservices in Docker Containern. Diese Anleitung basiert noch auf .NET
Core 2.2, es können aber die *FROM* Anweisungen im Dockerfile problemlos auf 3.0 aktualisiert werden:
```
FROM mcr.microsoft.com/dotnet/core/sdk:2.2  
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
```
