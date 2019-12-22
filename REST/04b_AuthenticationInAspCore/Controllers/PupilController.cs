using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]                       // Aktiviert die Authentication für den Controller.
    public class PupilController : ControllerBase
    {
        private static readonly List<string> _demopupils = new List<string> { "Pupil1", "Pupil2", "Pupil3" };

        // *****************************************************************************************
        // SZENARIO 1: Anonymer Zugriff erlaubt
        // *****************************************************************************************
        /// <summary>
        /// Reagiert auf GET Requests mit der URL api/pupil. Diese Route darf auch anonym, also ohne
        /// voriges Login aufgerufen werden.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]             // Jede Route ist geschützt, außer wir setzen AllowAnonymous
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public string Get()
        {
            return "This Information is for all Users.";
        }

        // *****************************************************************************************
        // SZENARIO 2: Zugriff mit (beliebigem) gültigen Token erlaubt
        // *****************************************************************************************

        /// <summary>
        ///  Reagiert auf GET Requests mit der URL api/pupil/me. Diese Route darf von allen
        ///  angemeldteten Usern besucht werden.
        /// </summary>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<string> GetMyData()
        {
            // Benutzername angemeldeten Users herausfinden.
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";
            // Die Rolle kann auch herausgesucht werden, ist hier aber nicht nötig.
            // string role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return Ok(_demopupils.FirstOrDefault(d => username.ToLower() == d.ToLower()));
        }

        // *****************************************************************************************
        // SZENARIO 3: Nur ein Token mit der eingetragenen Rolle Teacher ist erlaubt
        // *****************************************************************************************

        /// <summary>
        /// Reagiert auf GET Requests mit der URL /api/pupil/details. Diese Route darf nur von 
        /// Benutzern mit der Rolle Teacher (gesetzt mittels ClaimsIdentity.DefaultRoleClaimType in 
        /// UserService.GenerateToken()) aufgerufen werden.
        /// </summary>
        [Authorize(Roles = "Teacher")]             // Nur User der Rolle Teacher dürfen das sehen.
        [HttpGet("details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IEnumerable<string> GetDetails()
        {
            return _demopupils;
        }
    }
}