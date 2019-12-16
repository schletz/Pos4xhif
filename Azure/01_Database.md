# Einrichtung einer SQL Server Datenbank

## Vorbereitung

Lade das [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
für den Zugriff auf den SQL Server in Azure herunter.

## Anlegen der SQL Server Datenbank in Azure

Suche im Portal nach den Service *SQL Databases*. Im nachfolgenden Dialog kann mit *Add* eine
neue Datenbank angelegt werden.
![](add_sql_database.png)

Beim Anlegen muss die richtige Subscription gewählt werden (*Azure für Bildungseinrichtungen* ohne Starter).
Als Resource gruop wählen wir *AzureDemoApp*. Zu dieser Gruppe werden wir dann auch das App Service
hinzufügen.

> **Achtung**: Wähle bei *Compute + storage* den billigsten Plan (Basic). Beim voreingestellten Plan
> würde das Guthaben wenigen aufgebraucht sein!

![](configure_sql_server.png)

## Verbinden mit dem SQL Server Management Studio

Mit den bei *Create new server* eingegebenen Daten kannst du dich nun aus dem SQL Server Management
Studio (SSMS) verbinden.

- Servername: ???????.database.windows.net
- Authentication: SQL Server Authentication
- Login:    ?????
- Passwort: ?????

Beim ersten Verbinden fragt das Management Studio, ob die lokale IP Adresse zur Firewall hinzugefügt
werden soll. Dies muss natürlich gemacht werden, denn sonst ist keine Verbindung möglich.

### Anlegen eines weiteren Users für die Datenbank AzureDemo

In unserem Programm verwenden wir natürlich nicht den Admin, um sich zur Datenbank zu verbinden.
Deswegen legen wir einen weiteren Benutzer an, der in der Datenbank lediglich lesen und schreiben,
jedoch keine Änderungen am Schema vornehmen kann.

Dafür selektieren wir die Datenbank *AzureDemo* im SQL Server Management Studio und führen die
folgenden Befehle aus. Das Passwort muss Groß- und Kleinbuchstaben sowie Ziffern oder Sonderzeichen
enthalten.

![](ssms_add_user.png)

```sql
CREATE USER Demouser WITH PASSWORD = '?????';   -- Statt Demouser kommt der Username
ALTER ROLE db_datareader ADD MEMBER Demouser;   -- Statt Demouser kommt der Username
ALTER ROLE db_datawriter ADD MEMBER Demouser;   -- Statt Demouser kommt der Username
```
