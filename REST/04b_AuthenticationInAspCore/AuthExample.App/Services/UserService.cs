using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using AuthExample.App.Extensions;
using AuthExample.App.Model;

namespace AuthExample.App.Services
{
    /// <summary>
    /// Userservice für das Prüfen und Anlegen von Usern in der Datenbank.
    /// </summary>
    public class UserService
    {
        private readonly SchuleContext db;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="context">Wird über Depenency Injection durch services.AddDbContext() übergeben.</param>
        /// <param name="configuration">Wird über Depenency Injection übergeben.</param>
        public UserService(SchuleContext context, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.db = context;
        }

        /// <summary>
        /// Generiert den JSON Web Token für den übergebenen User.
        /// </summary>
        /// <param name="user">User aus dem HTTP Request Body.</param>
        /// <returns>
        /// Token, wenn der User Authentifiziert werden konnte. 
        /// Null wenn der Benutzer nicht gefunden wurde.
        /// </returns>
        public string GenerateToken(ApplicationUser user)
        {
            // Gibt es den User in der Datenbank?
            User dbUser = db.User.FirstOrDefault(u => u.U_Name.ToLower() == user.Username.ToLower());
            if (dbUser == null) { return null; }
            // Um das Passwort zu prüfen, berechnen wir den Hash mit dem Salt in der DB. Stimmt
            // das Ergebnis mit dem gespeichertem Ergebnis überein, ist das Passwort richtig.
            string hash = CalculateHash(user.Password, dbUser.U_Salt);
            if (hash != dbUser.U_Hash) { return null; }

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Convert.FromBase64String(configuration["AppSettings:Secret"]);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                // Payload für den JWT.
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // Benutzername als Typ ClaimTypes.Name.
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    // Rolle des Benutzer als ClaimTypes.DefaultRoleClaimType
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, dbUser.U_Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Erstellt einen neuen Benutzer in der Datenbank. Dafür wird ein Salt generiert und der
        /// Hash des Passwortes berechnet.
        /// Wird eine PupilId übergeben, so wird die Rolle "Pupil" zugewiesen, ansonsten "Teacher".
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public User CreateUser(ApplicationUser user)
        {
            string salt = GenerateSalt();
            User newUser = new User
            {
                U_Name = user.Username,
                U_Salt = salt,
                U_Hash = CalculateHash(user.Password, salt),
                U_Schueler_NrNavigation = db.Schueler.Find(user.PupilId)
            };
            // Zur Demonstrationszwecken weisen wir wenn keine Schülernummer zugewiesen wurde
            // die Rolle Teacher zu.
            newUser.U_Role = user.PupilId == null ? "Teacher" : "Pupil";
            db.Entry(newUser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            db.SaveChanges();
            return newUser;
        }

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
