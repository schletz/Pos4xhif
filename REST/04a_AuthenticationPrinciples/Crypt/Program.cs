using System;
namespace Crypt
{
    /// <summary>
    /// Demoprogramm für Hashalgorithmen. Start mit dotnet run.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DEMOPROGRAMM FÜR HASHING ALGORITHMEN");
            Console.WriteLine("====================================");
            Console.WriteLine();

            // Demo 1: Für manche Dinge braucht man einen zufällig generierten Base64 String
            //         (Secrets, ...)
            Console.Write("Anzahl der Bits für die Generierung eines Secrets: ");
            int bits = 128;
            try { bits = int.Parse(Console.ReadLine()) / 8 * 8; } catch { }
            string secret = GenerateSalt(bits);
            Console.WriteLine($"Generiertes Secret ({bits} bit, {secret.Length} Stellen):{Environment.NewLine}{secret}");
            Console.WriteLine();

            string password = "";
            do
            {
                // Demo 2: Hashen eines Passwortes. Das Secret wird generiert, dadurch haben gleiche
                //         Passwörter unterschiedliche Hashwerte.
                Console.Write("Passwort für den Hashvorgang (leer zum Beenden): ");
                password = Console.ReadLine();
                if (string.IsNullOrEmpty(password)) { break; }
                string salt = GenerateSalt(128);
                Console.WriteLine($"Generiertes Salt des Users (128bit): {salt}");
                string hashedPassword = CalculateHash(password, salt);
                Console.WriteLine($"Hashwert des Passwortes: {hashedPassword}");
                Console.WriteLine();
            }
            while (true);


            // Demo 3: Stimmt das Passwort "schueler" bei gegebenen Salt und Hashwert aus der Datenbank? 
            //         In der Datenbank stehen folgende Werte
            //         Salt: 1/xKujrQli/6jaiwwc91DA==
            //         Hashed Password: OudtCAJ511+tetcx1cZ3VXxUdmqwPeJn+ZehEZh0Fu4=
            if (CheckPassword("schueler", "1/xKujrQli/6jaiwwc91DA==", "OudtCAJ511+tetcx1cZ3VXxUdmqwPeJn+ZehEZh0Fu4="))
            {
                Console.WriteLine("Passwort richtig.");
            }
            else
            {
                Console.WriteLine("Passwort falsch.");

            }
            Console.WriteLine();
            Console.Write("ENTER zum Beenden.");
            Console.ReadLine();
        }

        /// <summary>
        /// Generiert eine Zufallszahl und gibt das Ergebnis Base64 Codiert zurück.
        /// </summary>
        /// <param name="bits">Länge der Zufallszahl, wird auf ganze Bytes abgerundet.</param>
        /// <returns>Base64 String mit der Länge bits * 4 / 3.</returns>
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
        /// UTF8 Werte der Zeichen die SHA256 Prüfsumme.
        /// </summary>
        /// <param name="password">Passwort, für welches der Hashwert berechnet wird.</param>
        /// <param name="salt">Salt, das für die Berechnung des Hashwertes verwendet wird.</param>
        /// <returns>Base64 Codierter String des SHA256 Hashwertes mit 44 Stellen Länge.</returns>
        private static string CalculateHash(string password, string salt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(salt))
            { return ""; }
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            System.Security.Cryptography.HMACSHA256 myHash = new System.Security.Cryptography.HMACSHA256(saltBytes);

            byte[] hashedData = myHash.ComputeHash(passwordBytes);

            // Das Bytearray wird Base64 codiert zurückgegeben.
            string hashedPassword = Convert.ToBase64String(hashedData);
            return hashedPassword;
        }
        /// <summary>
        /// Prüft, ob das übergebene Passwort mit dem gespeicherten Hashwert und dem Salt übereinstimmt.
        /// </summary>     
        static bool CheckPassword(string password, string salt, string hashedPassword) =>
            hashedPassword == CalculateHash(password, salt);

    }
}
