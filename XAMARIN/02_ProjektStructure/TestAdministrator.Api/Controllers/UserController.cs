using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAdministrator.Api.Services;
using TestAdministrator.Dto;

namespace TestAdministrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="userService">
        /// Wird über Dependency Injection durch services.AddScoped() in ConfigureJwt() übergeben.
        /// </param>
        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        /// <summary>
        /// POST Route für https://(url)/api/user/login. In Postman muss für https und 
        /// selbst signierte Zertifikate unter File - Settings - SSL certificate verifivation
        /// deaktiviert werden.
        /// </summary>
        /// <param name="user">User aus dem HTTP Request Body (RAW, Content type: JSON)</param>
        /// <returns>Token als String oder BadRequest wenn der Benutzer nicht angemeldet werden konnte.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody]UserDto user)
        {
            string token = userService.GenerateToken(user);

            // HTTP 401 liefern, wenn der User nicht authentifiziert werden kann.
            if (token == null)
                return Unauthorized();

            return Ok(token);
        }

    }
}