# Starten von Prozessen in ASP.NET Core

Es soll über den Windows Ping Befehl herausgefunden werden, wie viele ms das Pingpaket zu einem
Server benötigt. Dafür wird mit der Klasse *Process* der Prozess *ping.exe* mit den
entsprechenden Argumenten gestartet. Die Ausgabe in der Konsole sieht so aus (englische
Windows Version):

```
C:\Users\Michael>ping -n 10 www.google.com

Pinging www.google.com [142.251.36.196] with 32 bytes of data:
Reply from 142.251.36.196: bytes=32 time=18ms TTL=118
Reply from 142.251.36.196: bytes=32 time=17ms TTL=118
Reply from 142.251.36.196: bytes=32 time=13ms TTL=118
Reply from 142.251.36.196: bytes=32 time=13ms TTL=118
Reply from 142.251.36.196: bytes=32 time=13ms TTL=118
Reply from 142.251.36.196: bytes=32 time=13ms TTL=118
Reply from 142.251.36.196: bytes=32 time=12ms TTL=118
Reply from 142.251.36.196: bytes=32 time=13ms TTL=118
Reply from 142.251.36.196: bytes=32 time=13ms TTL=118
Reply from 142.251.36.196: bytes=32 time=13ms TTL=118

Ping statistics for 142.251.36.196:
    Packets: Sent = 10, Received = 10, Lost = 0 (0% loss),
Approximate round trip times in milli-seconds:
    Minimum = 12ms, Maximum = 18ms, Average = 13ms
```

## Das Service QueuedWorker

Zuerst wird das Service *QueuedWorker* in der Datei Program.cs registriert:

```c#
builder.Services.AddSingleton<QueuedWorker>(provider=>
    new QueuedWorker(
        provider.GetRequiredService<IServiceScopeFactory>(),
        provider.GetRequiredService<ILogger<QueuedWorker>>(),
        maxQueueLength: 10,
        maxProcesses: 2,
        timeout: 30000
));
```

Es kann durch 3 Parameter konfiguriert werden:

- **maxQueueLength:** Bestimmt, wie viele Jobs in der Warteschlange sein dürfen. Werden es
  mehr, so wird ein neuer Job abgelehnt und der User sieht eine Fehlermeldung.
- **maxProcesses:** Es werden, um schneller die Jobs abarbeiten zu können, mehrere PING
  Prozesse gestartet. Dieser Parameter bestimmt die gleichzeitig laufenden Prozesse. Bedenke, dass
  die Applikation im Internet vielleicht 100 User gleichzeitig bedienen. Daher ist eine
  Limitierung sehr wichtig.
- **timeout:** Nach diesem Zeitraum wird dem Prozess ein Kill Signal gesendet.

## Starten des Programmes

Führe *dotnet watch run* im Projektverzeichnis aus. Öffne danach den Browser an der angezeigten
URL. Es wird .NET 6 zur Ausführung benötigt.
