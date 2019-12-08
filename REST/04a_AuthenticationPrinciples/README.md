# Authentication und Authorization
Bis jetzt kann jeder, der unsere REST Schnittstelle aufruft, die Daten ansehen. Das ist natürlich
für echte Anwendungen nicht geeignet. Wir müssen uns daher mit 2 Mechanismen in ASP.NET Core
auseinandersetzen: Authentication und Authorization
> Authentication is the process to validate an anonymous user based on some credentials and 
> Authorization process happens just after that and grants resources to this validated user. 
> So, we can say, it's two-step validating process before providing the access of the resources 
> or data.
> (https://www.c-sharpcorner.com/article/authentication-and-authorization-in-asp-net-core-mvc-using-cookie/)

*Authentication* ist also das Login selbst (Prüfen von Benutzername und Passwort), *Authorization*
prüft im Controller, ob der Benutzer auch die Daten sehen oder ändern darf. ASP.NET bietet
Middleware an, um Benutzer zu authentifizieren (vgl. https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/identity-2x?view=aspnetcore-3.0):
- Cookie-based authentication
- JWT Bearer Authentication
- OpenID Connect (OIDC) authentication
- Facebook authentication
- Google authentication
- Microsoft Account authentication
- Twitter authentication


## Workflow: Authentication und Authorization
Die folgende Grafik zeigt 3 Szenarien, die oftmals abgebildet werden müssen. Zuerst registriert 
sich der Benutzer mit einem von ihm festgelegten Benutzernamen und Passwort. Vor dem Zugriff auf 
eine geschützte Ressource muss das Login validiert werden (Szenario 2). Der Benutzer erhält eine
kleine Information von unserem Server: den Token. Beim Abrufen einer Ressource (Szenario 3) muss
er den Token mitliefern. Ist dieser gültig und hat der Benutzer die Rechte auf diese Ressource, so
werden die Daten ausgeliefert.

![](AuthWorkflow.png)

## Richtige Speicherung des Passwortes in der Datenbank
Natürlich dürfen die Passwörter nicht im Klartext in der Datenbank gespeichert werden. Selbst wenn die Datenbank durch eine Lücke im System kopiert werden kann, darf aus diesen Daten kein 
Userlogin abgeleitet werden können.

Eine Möglichkeit, Passwörter sicher zu speichern, bieten *Hashalgorithmen*. Sie bilden den String
auf eine 256bit (SHA256) lange Zahl ab. Schon eine Stelle Unterschied im Passwort produziert einen
gänzlich anderen Hashwert (schwache Kausalität). Gleiche Passwörter werden auf den gleichen 
Hashwert abgebildet, und da liegt auch das Problem: Für oft gewählte einfache Passwörter (*1234*, *asdf*, ...) gibt es sogenannte *Rainbow Tables*. Hier kann der Hashwert auf eines dieser 
einfachen Passwörter zurückgeführt werden.

Die Lösung ist das sogenannte *Salt*. Beim Anlegen des Users wird ein z. B. 128bit (16 Byte)
langer zufälliger Wert generiert und gespeichert. Dieses Salt wird vor dem Berechnen des Hashwertes
des Passwortes einfach "angehängt". Man kann sich dies als Stringverknüpfung vorstellen, wo dann
SHA256(Salt + Passwort) berechnet wird. Da das Salt bei jedem Benutzer unterschiedlich ist, 
werden so auch einfache Passwörter auf immer unterschiedliche Werte abgebildet. Selbst wenn ein
Angreifer die Usertabelle zu Gesicht bekommt und somit das Salt weiß, muss er immer noch durch 
Probieren die Hashsummen berechnen und kann sie nicht mit einer Rainbow Table vergleichen.

### Salt und Hash in .NET
Natürlich bietet das .NET Framework entsprechende Methoden zum Berechnen des SHA256 Hashwertes.
Für das Berechnen des Hashwertes aus Passwort und Salt verwenden wir das *HMACSHA256* 
(Keyed-Hash Message Authentication Code) Verfahren. 
Würde Salt und Passwort nämlich wie vorher erwähnt mit einer einfachen Stringverknüpfung gebildet
werden, ergeben sich Kollisionen ("AB" + "CD" = "ABC" + "D"). Mit HMAC wird dieses Verknüpfen 
durch bitweise Operationen durchgeführt und ist daher besser als die Stringverknüpfung.

> HMACSHA256 is a type of keyed hash algorithm that is constructed from the SHA-256 hash function 
> and used as a Hash-based Message Authentication Code (HMAC). The HMAC process mixes a  secret key 
> with the message data, hashes the result with the hash function, mixes that hash value with the 
> secret key again, and then applies the hash function a second time. The output hash is 256 bits in 
> length.
> 
> An HMAC can be used to determine whether a message sent over an insecure channel has been tampered 
> with, provided that the sender and receiver share a secret key. The sender computes the hash value 
> for the original data and sends both the original data and hash value as a single message. The 
> receiver recalculates the hash value on the received message and checks that the computed HMAC 
> matches the transmitted HMAC.
> 
> Any change to the data or the hash value results in a mismatch, because knowledge of the secret 
> key is required to change the message and reproduce the correct hash value. Therefore, if the 
> original and computed hash values match, the message is authenticated. 
> (https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.hmacsha256?view=netframework-4.8)

Folgendes Codebeispiel generiert einen 128bit Salt Wert und berechnet den Hashwert des Passwortes
"1234":
```c#
using System;
namespace Crypt
{
    class Program
    {
        static void Main(string[] args)
        {
            // Salt generieren und als Base64 String speichern.
            string salt = GenerateSalt();

            // Nun wird das Passwort mit dem Salt versehen und der SHA256 Hashwert berechnet:
            string myPassword = "1234";
            string saltedPassword = CalculateHash(myPassword, salt);
        }

        /// <summary>
        /// Generiert eine 128bit lange Zufallszahl und gibt sie Base64Codiert (24 Stellen) in der Form
        /// nmU2xPjixsbAKqblq59NNg==
        /// zurück.
        /// </summary>
        private static string GenerateSalt()
        {
            // 128bit Salt erzeugen.
            byte[] salt = new byte[128 / 8];
            using (System.Security.Cryptography.RandomNumberGenerator rnd = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rnd.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Verknüpft das übergebene Salt mit dem Passwort und berechnet aufgrund der
        /// UTF8 Werte der Zeichen die SHA256 Prüfsumme. Gibt den Wert Base64 codiert (44 Stellen)
        /// in der Form 
        /// mswEwk+8U0rDGstvICGb5AhycUjw+si2PypAISs0U6Q=
        /// zurück.
        /// </summary>    
        private static string CalculateHash(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            System.Security.Cryptography.HMACSHA256 myHash = new System.Security.Cryptography.HMACSHA256(saltBytes);

            byte[] hashedData = myHash.ComputeHash(passwordBytes);

            // Das Bytearray wird Base64 codiert zurückgegeben.
            string hashedPassword = Convert.ToBase64String(hashedData);
            Console.WriteLine($"Salt:            {salt}");
            Console.WriteLine($"Password:        {password}");
            Console.WriteLine($"Hashed Password: {hashedPassword}");              
            return hashedPassword;
        }
        /// <summary>
        /// Prüft, ob das übergebene Passwort mit dem gespeicherten Hashwert und dem Salt übereinstimmt.
        /// </summary>     
        static bool CheckPassword(string password, string salt, string hashedPassword) =>
            hashedPassword == CalculateHash(password, salt);

    }
}
```

Wenn das Programm mehrmals gestartet wird, sehen wir, dass das gleiche Passwort durch den 
unterschiedlichen Wert von Salt ganz anders aussieht.
```
Salt:            ocdXKXQQHKAqiExuYc6lXQ==
Password:        1234
Hashed Password: u4Q4mtP6f4bAi01hhx57RYOON54xq7NpDMfsB9TJJlo=

Salt:            DvzbcNAOjLPW/NkphN++0w==
Password:        1234
Hashed Password: E7itIjT0wbzZtBEYrl4Ctbl7ydM5s64wfqllckUCFug=
```

### Base64 Codierung eines Bytearrays
In der Kryptografie entstehen oft Bytearrays. Binäre Übertragungen über HTTP zwischen 
unterschiedlichen Systemen sind jedoch schwierig, deswegen
wurde die *Base64* Repräsentation eingeführt. *Base64* bedeutet, dass pro 6 Bits ein ASCII 
Zeichen verwendet wird. Dies ergibt 64 verschiedene Zeichen. Diese Zeichen sind in allen 
Codierungen gleich und können daher systemübergreifend verwendet werden. Folgendes Bild zeigt die 
Umwandlung eines 3 Byte langen Arrays mit den Werten 45, 95 und 145:

![](Base64.png)

Ein Array aus n Bytes benötigt daher (n x 8) : 6 Zeichen und ist daher um 33% länger. Die Zeichentabelle ist normiert und 
kann z. B. auf [Wikipedia](https://de.wikipedia.org/wiki/Base64#Base64-Zeichensatz) nachgelesen 
werden.

### Aufbau der Tabelle User
Als Erweiterung der Schuldatenbank fügen wir eine Tabelle User ein. Diese Tabelle zeigt einige
Besonderheiten:
- Die interne User-ID darf nicht gesendet werden. Da es ein Autowert ist, kann ein Angreifer 
  annehmen, dass es nach der User-ID 13 eine User-ID 14 auch gibt.
- Der Benutzerame, den der Benutzer übermittelt und der auch im Token verwendet wird, ist 
  eindeutig (UNIQUE).
- Ein 128bit langer Wert für das Salt braucht 24 Stellen.
- Ein 256bit langer Wert für das den Hashwert des Passwortes braucht 44 Stellen.
- Die Schülernummer verweist - wenn der User ein Schüler ist - als Fremdschlüssel auf die 
  Schülertabelle.
- Die Rolle ist ein String und wird im nächsten Beispiel mit *Teacher* oder *Pupil* belegt.

```sql
CREATE TABLE User (
    U_ID                  INTEGER PRIMARY KEY AUTOINCREMENT,
    U_Name VARCHAR(100)   UNIQUE NOT NULL,
    U_Salt CHAR(24)       NOT NULL,   -- 128bit Salt, Base64 codiert
    U_Hash CHAR(44)       NOT NULL,   -- 256bit SHA, Base64 codiert
    U_Schueler_Nr         INTEGER,
    U_Role                VARCHAR(50),
    FOREIGN KEY (U_Schueler_Nr) REFERENCES Schueler(S_Nr)
);

CREATE UNIQUE INDEX idx_u_name      ON User (U_Name);
```

Die Tabelle würde dann - wenn sie jemand auslesen könnte - so aussehen:

| U_ID	| U_Name   	| U_Salt                  	| U_Hash                                      	| U_Schueler_Nr	| U_Role 	| 
| ----	| ---------	| ------------------------	| --------------------------------------------	| -------------	| -------	| 
| 9   	| Lehrer1  	| noqXFYS7VL5wqxmbOv41OQ==	| C4Tt67lyuAWSVNPFZwtDCuBsXFGB4RR7f7qszW1PJl0=	|              	| Teacher	| 
| 10  	| Lehrer2  	| FglPztqTfpjd73CpXPA6Og==	| itabYOk1pZkJMCGmnlA4wH9NSvX4RUidW3t0ONiyZjM=	|              	| Teacher	| 
| 11  	| Schueler1	| s0dhx6/xuNC74HCX5q4rRA==	| hllpA1FpezGSQoZ6Wpnv14tccH2RB15yfsY3H9B1m3A=	| 1001         	| Pupil  	| 
| 12  	| Schueler2	| mkDTubveMcrkEZBcN6nMag==	| hkV69pMysXr6YkTW8M/YXnGWBrijGfbxvTPcEyS7UQ8=	| 2001         	| Pupil  	| 


## JSON Web Token
Eine REST API arbeitet *stateless*. Das bedeutet, dass die gleiche Anfrage unabhängig von der
Vorgeschichte gleich beantwortet wird. Ein Login widerspricht dem zunächst einmal. Es kommt ja
darauf an, ob sich der Benutzer vorher angemeldet hat. Daher schickt unser Server dem Benutzer 
beim Login einen sogenannten *Token*.

Im einfachsten Fall ist dieser Token eine lange zufällige Zeichenfolge. Übermittelt uns ein
Benutzer den Token, können wir z. B. in einer Stringliste nachsehen, ob wir diesen generiert 
haben und ob er noch gültig ist. Das hat allerdings einige Nachteile:
- Es ist nicht wirklich stateless. Startet der Server neu, gehen die ausgestellten Token verloren.
- Speichern wir diese Token in einer Datenbank, müssen wir ständig auf diese zwecks Prüfung 
  zugreifen. Außerdem müssen Zusatzinformationen (Welcher Benutzer ist mit dem Token verknüpft?
  Welche Rolle hat er im System?) erst gelesen werden.

Um diese Probleme zu umgehen, wurde der JSON Web Token (JWT) geschaffen. Dieser bietet folgende
Funktionen:
- Beim Ausstellen können Nutzdaten (der Payload) wie Benutzername, Rolle, ... verpackt werden.
- Durch das Berechnen eines Hashwerts mit einem Secret, das nur dem Server bekannt ist, kann der
  User diesen Payload nicht manipulieren.
- Auch nach dem Neustart des Servers bleibt der Token durch den Payload gültig.

Ein aus dem folgenden Beispiel erzeugter Token sieht z. B. so aus:
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IlByb2Zlc3NvciIsInJvbGUiOiJUZWFjaGVyIiwibmJmIjoxNTcwOTc0NTU4LCJleHAiOjE1NzE1NzkzNTgsImlhdCI6MTU3MDk3NDU1OH0.LMQiNt96oMcjO2vwGm07rr5G-UCmSrGPWsMxE9wxeuw
```

Er besteht aus 3 Teilen, die mit . verknüpft sind. Eine Decodierung kann auf z. B. auf [jwt.io](https://jwt.io/) durchgeführt werden:

![](JwtDecode.png)

Was passiert, wenn der Benutzer nun den Payload ändert und die Daten wieder Base64 codiert 
zurücksendet? Da wir beim Erstellen des letzten Teiles - der Signatur - ein Secret am Server
verwendet haben, kann der Benutzer diese Signatur nicht neu berechnen. Beim Prüfen der Signatur
fällt daher die Manipulation auf und der Token wird verworfen.

Im folgenden Beispiel ist das Secret eine über [PuTTYgen](https://www.chiark.greenend.org.uk/~sgtatham/putty/latest.html) generierte Zeichenfolge:
```
AAAAB3NzaC1yc2EAAAABJQAAAQEAtiZcYpez2uCs9Zz/b+Ct4gSD5oxSmIsqOBxFR53/Wx2brYlbII69+Zo/iXtx2sR7YHIzScywxXzRplFv43b8aEJORL0Nhc5zn42oB+ZlTCF+xXiNrjwDI8tNSnEYT/lot3+tDG0uW0duvuOMOfADCBtDxQQQzMtY52dpGhkKhXgQgixGIT2eDVaeY006KENQYybETFXwWKu9YUsKOgTlo39wvU5EZ0lzcLnYTNxEjoNoXCetgYT+tPdbnTxozYsVD3an0ZQF9R8HFgVw2r3Xzos6PVxjFvpynDVL0oV5V9EPLfKkPU8H+CIc172TT5FwuU+L1Hrdeo6BvG1DB47jBQ
```
