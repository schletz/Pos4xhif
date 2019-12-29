using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationDemo.Services
{
    /// <summary>
    /// TODO: Die Methode CheckUserAndGetRole() anpassen. Eventuell ist noch der DbContext
    /// im Konstruktor zu ergänzen, damit das Service Zugriff auf die Datenbank hat.
    /// </summary>
    public class AuthService
    {
        private readonly byte[] _secret = new byte[0];

        /// <summary>
        /// Konstruktor für die Verwendung ohne JWT.
        /// </summary>
        public AuthService()
        {
        }

        /// <summary>
        /// Konstruktor mit Secret für die Verwendung mit JWT.
        /// </summary>
        /// <param name="secret">Base64 codierter String für das Secret des JWT.</param>
        public AuthService(string secret)
        {
            if (string.IsNullOrEmpty(secret))
            {
                throw new ArgumentException("Secret is null or empty.", nameof(secret));
            }
            _secret = Convert.FromBase64String(secret);
        }

        /// <summary>
        /// Prüft, ob der übergebene User existiert und gibt seine Rolle zurück.
        /// TODO: Anpassen der Logik an die eigenen Erfordernisse.
        /// </summary>
        /// <param name="credentials">Benutzername und Passwort, die geprüft werden.</param>
        /// <returns>
        /// Rolle, wenn der Benutzer authentifiziert werden konnte.
        /// Null, wenn der Benutzer nicht authentifiziert werden konnte.
        /// </returns>
        protected virtual async Task<string> CheckUserAndGetRole(UserCredentials credentials)
        {
            #region Variablen zur Demonstration. Sind zu entfernen.
            List<string> users = new List<string> { "pupil1", "teacher1" };
            /// Fixes Salt zur Demonstration. Ist im Code durch das generierte Salt pro User zu ersetzen.
            string salt = "5Snh3qZNODtDd2Ibsj7irayIl6E1WWmpbvXtcSGlm1o=";
            /// Fixes Passwort zur Demonstration.
            string password = "1234";
            /// Fixer Hash zur Demonstration.
            string hashedPassword = CalculateHash(password, salt);
            #endregion

            // TODO: Abfragen, ob es den User überhaupt gibt
            if (!users.Any(u => u == credentials.Username)) { return null; }

            // TODO: Um das Passwort zu prüfen, berechnen wir den Hash mit dem Salt in der DB. Stimmt
            // das Ergebnis mit dem gespeichertem Ergebnis überein, ist das Passwort richtig.
            string hash = CalculateHash(credentials.Password, salt);
            if (hash != hashedPassword) { return null; }

            // TODO: Die echte Rolle aus der DB lesen oder ermitteln.
            return credentials.Username.StartsWith("teacher") ? "Teacher" : "Pupil";
        }

        /// <summary>
        /// Erstellt einen neuen Benutzer in der Datenbank. Dafür wird ein Salt generiert und der
        /// Hash des Passwortes berechnet.
        /// Wird eine PupilId übergeben, so wird die Rolle "Pupil" zugewiesen, ansonsten "Teacher".
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        /*
        public async Task<User> CreateUser(UserCredentials credentials)
        {
            string salt = GenerateRandom();
            // Den neuen Userdatensatz erstellen
            User newUser = new User
            {
                U_Name = credentials.Username,
                U_Salt = salt,
                U_Hash = CalculateHash(credentials.Password, salt),
            };
            // Die Rolle des Users zuweisen
            newUser.U_Role = "";
            db.Entry(newUser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            await db.SaveChangesAsync();
            return newUser;
        }
        */

        /// <summary>
        /// Liest die Details des übergebenen Users aus der Datenbank.
        /// </summary>
        /// <param name="userid">Username, nach dem gesucht wird.</param>
        /// <returns>Userobjekt aus der Datenbank</returns>
        /*
        public Task<User> GetUserDetails(string userid)
        {
            
        }
        */

        public Task<string> GenerateToken(UserCredentials credentials)
        {
            return GenerateToken(credentials, TimeSpan.FromDays(7));
        }

        /// <summary>
        /// Generiert den JSON Web Token für den übergebenen User.
        /// </summary>
        /// <param name="credentials">Userdaten, die in den Token codiert werden sollen.</param>
        /// <returns>
        /// JSON Web Token, wenn der User Authentifiziert werden konnte. 
        /// Null wenn der Benutzer nicht gefunden wurde.
        /// </returns>
        public async Task<string> GenerateToken(UserCredentials credentials, TimeSpan lifetime)
        {
            if (credentials is null) { throw new ArgumentNullException(nameof(credentials)); }

            string role = await CheckUserAndGetRole(credentials);
            if (role == null) { return null; }

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
                Expires = DateTime.UtcNow + lifetime,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_secret),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Erstellt für den User ein ClaimsIdentity Objekt, wenn der User angemeldet werden konnte.
        /// </summary>
        /// <param name="credentials">Username und Passwort, welches geprüft werden soll.</param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateIdentity(UserCredentials credentials)
        {
            string role = await CheckUserAndGetRole(credentials);
            if (role != null)
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, credentials.Username),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, role)
                };
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                    claims,
                    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                return claimsIdentity;
            }
            return null;
        }

        /// <summary>
        /// Generiert eine Zufallszahl und gibt sie Base64 codiert zurück.
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandom(int length = 128)
        {
            // Salt erzeugen.
            byte[] salt = new byte[length / 8];
            using (System.Security.Cryptography.RandomNumberGenerator rnd =
                System.Security.Cryptography.RandomNumberGenerator.Create())
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
        protected static string CalculateHash(string password, string salt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(salt))
            {
                throw new ArgumentException("Invalid Salt or Passwort.");
            }
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            System.Security.Cryptography.HMACSHA256 myHash =
                new System.Security.Cryptography.HMACSHA256(saltBytes);

            byte[] hashedData = myHash.ComputeHash(passwordBytes);

            // Das Bytearray wird als Hexstring zurückgegeben.
            string hashedPassword = Convert.ToBase64String(hashedData);
            return hashedPassword;
        }
    }

}
