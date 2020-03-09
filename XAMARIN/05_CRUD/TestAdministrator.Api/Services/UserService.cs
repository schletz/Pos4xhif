using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TestAdministrator.Api.Model;
using TestAdministrator.Dto;

namespace TestAdministrator.Api.Services
{
    /// <summary>
    /// Registiert neue Benutzer und verifiziert Benutzerlogins. Generiert JWT Strings bei
    /// verifizierten Benutzern.
    /// </summary>
    public class UserService
    {
        private readonly TestsContext db;
        private readonly IConfiguration configuration;
        /// <summary>
        /// Bestimmt die Dauer Gültigkeit des Tokens.
        /// </summary>
        public TimeSpan ExpirationTime => new TimeSpan(24, 0, 0);

        /// <summary>
        /// Konstruktor. Setzt die ASP.NET Konfiguration und den DB Context.
        /// </summary>
        /// <param name="context">Wird über Depenency Injection durch services.AddDbContext() übergeben.</param>
        /// <param name="configuration">Wird über Depenency Injection von ASP.NET übergeben.</param>
        public UserService(TestsContext context, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.db = context;
        }

        /// <summary>
        /// Generiert den JSON Web Token für den übergebenen User, wenn dieser authentifiziert
        /// werden konnte.
        /// </summary>
        /// <param name="user">User, der verifiziert werden soll.</param>
        /// <returns>
        /// Token, wenn der User Authentifiziert werden konnte. 
        /// Null wenn der Benutzer nicht gefunden wurde.
        /// </returns>
        public UserDto GenerateToken(UserDto user)
        {
            // TODO:
            // 1. Prüfen des Usernamens
            User dbUser = db.Users.Find(user.Username);
            if (dbUser == null) { return null; }

            // 2. Lesen des salt aus der DB
            // 3. Stimmt CalculateHash(user.Password, salt) mit dem Hash der DB überein?
            if (CalculateHash(user.Password, dbUser.Salt) != dbUser.Hash)
            { return null; }
            else
            { user.Password = ""; }

            // 4. Die Rolle aus der DB lesen oder bestimmen. Außerdem wird die ID des Users
            //    (Kürzel oder Schülernummer) gesetzt.
            user.Role = UserDto.Userrole.Pupil;
            if (db.Teacher.Any(t => t.T_Account == user.Username))
            { 
                user.Role = UserDto.Userrole.Teacher;
                user.TeacherId = db.Teacher.SingleOrDefault(t => t.T_Account == user.Username)?.T_ID;
            }
            else
            {
                user.PupilId = db.Pupil.SingleOrDefault(p => p.P_Account == user.Username)?.P_ID;
            }

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            // Aus appsettings.json das Secret lesen.
            byte[] key = Convert.FromBase64String(configuration["AppSettings:Secret"]);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                // Payload für den JWT.
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // Benutzername als Typ ClaimTypes.Name.
                    new Claim(ClaimTypes.Name, user.Username),
                    // Rolle des Benutzer als ClaimTypes.DefaultRoleClaimType setzen.
                    // ACHTUNG: Muss mit den Annotations der Routen in [Authorize(Roles = "xxx")]
                    // übereinstimmen.
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString().ToLower())
                }),
                Expires = DateTime.UtcNow + ExpirationTime,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            // Den Zeitpunkt des Logins in die DB schreiben.
            dbUser.LastLogin = DateTime.UtcNow;
            return user;
        }

        /// <summary>
        /// Erstellt einen neuen Benutzer in der Datenbank. Dafür wird ein Salt generiert und der
        /// Hash des Passwortes berechnet.
        /// </summary>
        /// <param name="user">Der Benutzer, der in der Datenbank angelegt werden soll.</param>
        /// <returns>Userobjekt, welches in der Datenbank angelegt wurde.</returns>
        public void CreateUser(UserDto user)
        {
            string salt = GenerateSalt();
            string hash = CalculateHash(user.Password, salt);
            // TODO: 
            // 1. User in die DB schreiben.
            // 2. Das Userobjekt (Modelklasse) statt void zurückgeben.
            db.Users.Add(new User
            {
                Name = user.Username,
                Salt = salt,
                Hash = hash
            });
            db.SaveChanges();
        }

        /// <summary>
        /// Generiert eine Zufallszahl und gibt diese Base64 codiert zurück.
        /// </summary>
        /// <param name="bits">Anzahl der Bits der Zufallszahl.</param>
        /// <returns>Base64 String mit der Länge bits * 4 / 3.</returns>
        private static string GenerateSalt(int bits = 128)
        {
            byte[] salt = new byte[bits / 8];
            // Kryptografisch sichere Zufallszahlen erzeugen.
            using (System.Security.Cryptography.RandomNumberGenerator rnd = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rnd.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Berechnet den HMACSHA256 Wert des übergebenen Passwortes.
        /// </summary>
        /// <param name="password">Passwort, welches codiert werden soll.</param>
        /// <param name="salt">Salt, mit dem der Hashwert berechnet werden soll.</param>
        /// <returns>Base64 codierter String mit 44 Stellen Länge.</returns>
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

            // Das Bytearray wird als Base64 String zurückgegeben.
            string hashedPassword = Convert.ToBase64String(hashedData);
            return hashedPassword;
        }
    }
}
