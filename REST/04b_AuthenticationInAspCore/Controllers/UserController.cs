using AuthenticationDemo.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationDemo.Controllers
{
    /// <summary>
    /// Controller zum Aufrufen der Authentication Services:
    /// Für den Einsatz in WebAPI Projekten:
    ///     user/login: Prüft die Userdaten aus dem Request Body und gibt einen JWT zurück.
    /// Für den Einsatz in Blazor oder MVC Projekten:
    ///     user/loginform: Prüft die x-www-formencoded codierten Userdaten, setzt das Cookie und
    ///                     leitet danach auf die Standardseite (/) um.
    ///     user/logout: Löscht das Cookie und leitet danach auf die Standardseite (/) um.
    ///     
    /// user/register: Liest die Daten aus dem Registrierungsformular und legt den User in der DB an.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Setzt das AuthService, welches mit
        ///     services.AddScoped<UserService>(services => 
        ///         new UserService(jwtSecret);
        /// in ConfigureServices() registriert wurde.
        /// </summary>
        /// <param name="userService">Registriertes Userservice.</param>
        public UserController(AuthService authService, ILogger<UserController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// POST Route für /api/user/login
        /// Generiert den Token für eine JWT basierende Autentifizierung.
        /// </summary>
        /// <param name="user">Userdaten aus dem HTTP Request Body (RAW, Content type: JSON)</param>
        /// <returns>Token als String oder BadRequest wenn der Benutzer nicht angemeldet werden konnte.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<string>> Login(UserCredentials user)
        {
            string token = await _authService.GenerateToken(user);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }

        /// <summary>
        /// POST Route für /api/user/loginform
        /// Setzt das Cookie für die Authentifizierung, wenn der User angemeldet werden konnte.
        /// Leitet danach auf die Standardseite (/) um.
        /// </summary>
        /// <param name="user">Userdaten aus dem Loginformular.</param>
        /// <returns>Redirect zur Hauptseite.</returns>
        [HttpPost("loginform")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public async Task<ActionResult<string>> LoginAsync([FromForm] UserCredentials user)
        {
            await HttpContext
                .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            ClaimsIdentity claimsIdentity;
            if ((claimsIdentity = await _authService.GenerateIdentity(user)) != null)
            {
                // Spezielle Properties (Expires, ...) können als 3. Parameter mit einer
                // AuthenticationProperties Instanz übergeben werden.
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));
            }
            return Redirect("/");
        }

        /// <summary>
        /// POST Route für /api/user/logout
        /// Entfernt das Cookie für die Authentifizierung.
        /// </summary>
        /// <returns>Redirect zur Hauptseite.</returns>
        [HttpGet("logout")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext
                .SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }

        /// <summary>
        /// Erstellt einen Benutzer in der Datenbank und gibt den erstellten Benutzer zurück.
        /// </summary>
        /// <param name="user">Daten aus dem Registrierungsformular, die angelegt werden.</param>
        /// <returns></returns>
        /*
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<User>> Register(UserDto user)
        {
            User newUser;
            try
            {
                newUser = await _authService.CreateUser(user);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                return Conflict();
            }
            return Ok(newUser);
        }
        */
    }
}