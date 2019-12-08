using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthExample.App.Model;
using AuthExample.App.Services;

namespace AuthExample.App.Controllers
{
    /// <summary>
    /// Controller für das Login und Anlegen von Benutzern.
    /// </summary>
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
        /// POST Route für /api/user/login
        /// </summary>
        /// <param name="user">User aus dem HTTP Request Body (RAW, Content type: JSON)</param>
        /// <returns>Token als String oder BadRequest wenn der Benutzer nicht angemeldet werden konnte.</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody]ApplicationUser user)
        {
            string token = userService.GenerateToken(user);

            if (token == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(token);
        }

        /// <summary>
        /// Erstellt einen Benutzer in der Datenbank. Wird eine PupilId übergeben, so wird die Rolle 
        /// "Pupil" zugewiesen, ansonsten "Teacher".
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]ApplicationUser user)
        {
            User newUser;
            try
            {
                newUser = userService.CreateUser(user);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { e.Message, Inner = e.InnerException?.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new
            {
                newUser.U_Name,
                newUser.U_Schueler_Nr,
                newUser.U_Role
            });
        }
    }
}