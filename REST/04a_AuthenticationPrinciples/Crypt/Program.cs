﻿using System;
namespace Crypt
{
    /// <summary>
    /// Demoprogramm für Hashalgorithmen. Start mit dotnet run.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GenerateSalt(1024));
            
            // Passwortüberprüfung: Stimmt das Kennwort "schueler" bei folgenden Daten in der db:
            //        Salt: 1IL3xJaVSj72xAI+T5etQg==
            //      Hashed Password: zujBw+E5qb5GlQnKmyXh0TtBzmEFOX3cwNRK8sf2yoU=

            if (CalculateHash("schueler", "1bZuvLSqvRR2D50UjucQJA==") == "Zb/bmd2KjqRJq6KCT4uxi34q2IA8H5xUsBF2lE2d9t8=")
            {
                Console.WriteLine("RICHTIG!!");
            }


        }

        /// <summary>
        /// Generiert eine 128bit lange Zufallszahl und gibt sie Base64Codiert (24 Stellen) in der Form
        /// nmU2xPjixsbAKqblq59NNg==
        /// zurück.
        /// </summary>
        private static string GenerateSalt(int bits = 128)
        {
            // 128bit Salt erzeugen.
            byte[] salt = new byte[bits / 8];
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
