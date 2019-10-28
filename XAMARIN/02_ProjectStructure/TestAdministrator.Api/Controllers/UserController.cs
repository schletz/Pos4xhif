using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAdministrator.Api.Services;
using TestAdministrator.Dto;

namespace TestAdministrator.Api.Controllers
{
    /// <summary>
    /// Bearbeitet Loginrequests.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;

        /// <summary>
        /// Konstruktor. Setzt das Userservice.
        /// </summary>
        /// <param name="userService">
        /// Userservice, welches mit services.AddScoped() in ConfigureJwt() gesetzt wurde und die
        /// Methoden zum Verifizieren des Logins anbietet.
        /// </param>
        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// POST /api/user/login. 
        /// Generiert den JWT Token und sendet ihn dem Benutzer.
        /// In Postman muss für https und selbst signierte Zertifikate unter 
        /// File - Settings - SSL certificate verification
        /// deaktiviert werden.
        /// </summary>
        /// <param name="user">User aus dem HTTP Request Body (RAW, Content type: JSON)</param>
        /// <returns>
        /// HTTP 200: Token als JSON Objekt mit dem Aufbau {token: string}.
        /// HTTP 401: Nicht authentifiziert. Der Benutzername oder das Kennwort stimmen nicht.
        /// </returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody]UserDto user)
        {
            string token = userService.GenerateToken(user);

            // HTTP 401 liefern, wenn der User nicht authentifiziert werden kann.
            if (token == null)
                return Unauthorized();
            user.Token = token;
            user.Password = "";
            return Ok(user);
        }

        /// <summary>
        /// Ein "echtes" Logout kann es nicht geben, da wir stateless sind. Dem Benutzer gehört der 
        /// Token, daher können wir ihn nicht "ungültig" machen. 
        /// Manchmal muss man aber etwas bereinigen, daher dieser Mustercode, wie man den 
        /// angemelteten Benutzer identifizieren kann.
        /// </summary>
        /// <returns></returns>
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            string role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            // TODO: Ressourcen bereinigen.
            return Ok();
        }
    }
}