using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationDemo.Model;
using AuthenticationDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// POST Route für /api/user/login
        /// </summary>
        /// <param name="user">User aus dem HTTP Request Body (RAW, Content type: JSON)</param>
        /// <returns>Token als String oder BadRequest wenn der Benutzer nicht angemeldet werden konnte.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<string> Login(UserCredentials user)
        {
            string token = _userService.GenerateToken(user);

            if (token == null)
                return Unauthorized();

            return Ok(token);
        }

        /// <summary>
        /// Erstellt einen Benutzer in der Datenbank und gibt den erstellten Benutzer zurück.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /*
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public ActionResult<User> Register(UserDto user)
        {
            User newUser;
            try
            {
                newUser = _userService.CreateUser(user);
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