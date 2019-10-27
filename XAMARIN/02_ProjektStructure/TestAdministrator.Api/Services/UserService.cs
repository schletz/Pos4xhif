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
    public class UserService
    {
        private readonly TestsContext db;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="context">Wird über Depenency Injection durch services.AddDbContext() übergeben.</param>
        /// <param name="configuration">Wird über Depenency Injection übergeben.</param>
        public UserService(TestsContext context, IConfiguration configuration)
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
        public string GenerateToken(UserDto user)
        {
            // TODO:
            // 1. Prüfen des Usernamens
            // 2. Lesen des salt aus der DB
            // 3. Stimmt CalculateHash(user.Password, salt) mit dem Hash der DB überein?
            // 4. Die Rolle aus der DB lesen oder bestimmen.
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
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, "Teacher")
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
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public void CreateUser(UserDto user)
        {
            string salt = GenerateSalt();
            string hash = CalculateHash(user.Password, salt);
            // TODO: 
            // 1. User in die DB schreiben.
            // 2. Das Userobjekt (Modelklasse) statt void zurückgeben.
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
