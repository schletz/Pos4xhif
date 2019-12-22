using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using AuthenticationDemo.Model;


namespace AuthenticationDemo.Services
{
    public class UserService
    {
        #region Properties zur Demonstration. Sind zu entfernen.
        private static List<string> Users { get; } = new List<string> { "pupil1", "teacher1" };
        /// <summary>
        /// Fixes Salt zur Demonstration. Ist im Code durch das generierte Salt pro User zu ersetzen.
        /// </summary>
        private static string Salt { get; } = "5Snh3qZNODtDd2Ibsj7irayIl6E1WWmpbvXtcSGlm1o=";
        /// <summary>
        /// Fixes Passwort zur Demonstration.
        /// </summary>
        private static string Password { get; } = "1234";
        /// <summary>
        /// Fixer Hash zur Demonstration.
        /// </summary>
        private static string HashedPassword { get; } = CalculateHash(Password, Salt);
        #endregion

        private readonly byte[] _secret;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="configuration">Liest das Secret aus appsettings.json. 
        /// Wird über Depenency Injection übergeben.</param>
        public UserService(string secret)
        {
            // Das Secret aus der appsettings.json lesen.
            _secret = Convert.FromBase64String(secret);
        }

        /// <summary>
        /// Generiert den JSON Web Token für den übergebenen User.
        /// </summary>
        /// <param name="credentials">User aus dem HTTP Request Body.</param>
        /// <returns>
        /// Token, wenn der User Authentifiziert werden konnte. 
        /// Null wenn der Benutzer nicht gefunden wurde.
        /// </returns>
        public string GenerateToken(UserCredentials credentials)
        {
            // TODO: Abfragen, ob es den User überhaupt gibt
            if (!Users.Any(u => u == credentials.Username)) { return null; }

            // TODO: Um das Passwort zu prüfen, berechnen wir den Hash mit dem Salt in der DB. Stimmt
            // das Ergebnis mit dem gespeichertem Ergebnis überein, ist das Passwort richtig.
            string hash = CalculateHash(credentials.Password, Salt);
            if (hash != HashedPassword) { return null; }

            // TODO: Die echte Rolle aus der DB lesen oder ermitteln.
            string role = credentials.Username.StartsWith("teacher") ? "Teacher" : "Pupil";

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                // Payload für den JWT.
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // Benutzername als Typ ClaimTypes.Name.
                    new Claim(ClaimTypes.Name, credentials.Username.ToString()),
                    // Rolle des Benutzer als ClaimTypes.DefaultRoleClaimType
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Erstellt einen neuen Benutzer in der Datenbank. Dafür wird ein Salt generiert und der
        /// Hash des Passwortes berechnet.
        /// Wird eine PupilId übergeben, so wird die Rolle "Pupil" zugewiesen, ansonsten "Teacher".
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        /*
        public User CreateUser(UserDto userdata)
        {
            string salt = GenerateSalt();
            // Den neuen Userdatensatz erstellen
            User newUser = new User
            {
                U_Name = userdata.Username,
                U_Salt = salt,
                U_Hash = CalculateHash(userdata.Password, salt),
            };
            // Die Rolle des Users zuweisen
            newUser.U_Role = "";
            db.Entry(newUser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            db.SaveChanges();
            return newUser;
        }
        */

        /// <summary>
        /// Generiert eine 128bit lange Zufallszahl und gibt sie Base64 codiert zurück.
        /// </summary>
        /// <returns></returns>
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
        /// Berechnet den HMACSHA256 Wert des Passwortes mit dem übergebenen Salt.
        /// </summary>
        /// <param name="password">Base64 Codiertes Passwort.</param>
        /// <param name="salt">Base64 Codiertes Salt.</param>
        /// <returns></returns>
        private static string CalculateHash(string password, string salt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(salt))
            {
                throw new ArgumentException("Invalid Salt or Passwort.");
            }
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            System.Security.Cryptography.HMACSHA256 myHash = new System.Security.Cryptography.HMACSHA256(saltBytes);

            byte[] hashedData = myHash.ComputeHash(passwordBytes);

            // Das Bytearray wird als Hexstring zurückgegeben.
            string hashedPassword = Convert.ToBase64String(hashedData);
            return hashedPassword;
        }
    }

}
