using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthExample.App.Model;
using AuthExample.App.Services;

namespace AuthExample.App.Controllers
{
    /// <summary>
    /// Controller für Pupil.
    /// </summary>
    [Authorize]                      // Legt fest, dass alle Routen standardmäßig geschützt sind.
    [Route("api/[controller]")]      // Reagiert auf /api/pupil
    [ApiController]
    public class PupilController : ControllerBase
    {
        private readonly SchuleContext db;
        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="context">
        /// Wird durch Depenency Injection in services.AddDbContext() 
        /// in ConfigureDatabase() übergeben.
        /// </param>
        public PupilController(SchuleContext context)
        {
            this.db = context;
        }

        /// <summary>
        /// Reagiert auf GET Requests mit der URL api/pupil. Diese Route darf auch anonym, also ohne
        /// voriges Login aufgerufen werden.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            string role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return Ok(db.Schueler.Select(s => new { s.S_Vorname, s.S_Zuname }));
        }

        /// <summary>
        ///  Reagiert auf GET Requests mit der URL api/pupil/myclass. Diese Route darf nur 
        ///  authorisiert benutzt werden. Es werden nur die Schüler in der Klasse des angemeldeten
        ///  Users zurückgegben.
        /// </summary>
        /// <returns></returns>
        [HttpGet("myclass")]
        public IActionResult GetUserClass()
        {
            // Benutzername und Rolle des angemeldeten Users herausfinden.
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            string role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;


            User dbUser = db.User
                .Include(u => u.U_Schueler_NrNavigation)
                .FirstOrDefault(u => u.U_Name == username);
            string userKlasse = dbUser?.U_Schueler_NrNavigation?.S_Klasse ?? "";
            return Ok(
                db.Schueler
                .Where(s => s.S_Klasse == userKlasse)
                .Select(s => new {s.S_Nr, s.S_Vorname, s.S_Zuname, s.S_Geschl})
            );
        }

        /// <summary>
        /// Reagiert auf GET Requests mit der URL /api/pupil/details. Liefert die Details zu allen
        /// Schülern. Diese Route darf nur von Benutzern mit der Rolle Teacher (gesetzt mittels
        /// ClaimsIdentity.DefaultRoleClaimType in UserService.GenerateToken() aufgerufen werden.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Teacher")]
        [HttpGet("details")]
        public IActionResult GetDetails()
        {
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            string role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return Ok(
                db.Schueler
                .Select(s => new { s.S_Nr, s.S_Vorname, s.S_Zuname, s.S_Geschl, s.S_Klasse })
            );
        }

        /// <summary>
        /// Reagiert GET Requests mit der URL api/pupil/1001. Diese Route darf nur von authorisierten
        /// Benutzern verwendet werden.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetId(string id)
        {
            if (!long.TryParse(id, out long idParsed))
            {
                return Ok();            // Alternativ: return BadRequest();
            }
            return Ok(
                db.Schueler
                .Where(s => s.S_Nr == idParsed)
                .Select(s => new { s.S_Nr, s.S_Vorname, s.S_Zuname, s.S_Geschl, s.S_Klasse })
            );
            // Braucht [JsonIgnore] über den Navigation Properties in den Modelklassen, sonst werden
            // auch alle Properties der Klasse ausgegeben. Besser ist mit new ein DTO Objekt 
            // auszugeben.
        }

        /// <summary>
        /// Legt einen neuen Schüler in der Datenbank an. Reagiert auf POST Requests mit der URL /api/pupil/rawdata.
        /// Die Daten müssen in der Form raw als application/json übermittelt werden.
        /// Daher ist in Postman unter Body "raw" und "JSON" einzustellen.
        /// Diese Route darf nur von Lehrern verwendet werden.
        /// Daten:
        /// {
        ///	"S_Nr": 1,
        ///	"S_Klasse": "5AHIF",
        ///	"S_Zuname": "Mustermann",
        ///	"S_Vorname": "Max",
        ///	"S_Geschl": "m"
        /// }
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public IActionResult PostFromBody([FromBody] Schueler schueler)
        {
            db.Entry(schueler).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            try
            {
                db.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new { schueler.S_Nr, schueler.S_Vorname, schueler.S_Zuname, schueler.S_Geschl, schueler.S_Klasse });
        }

        /// <summary>
        /// Reagiert auf DELETE Requests mit der URL /api/pupil/id. Darf nur von Lehrern verwendet werden.
        /// </summary>
        [Authorize(Roles = "Teacher")]
        [HttpDelete("{id}")]
        public IActionResult DeleteFromForm(string id)
        {
            if (!long.TryParse(id, out long idParsed)) { return BadRequest(); }

            Schueler found = db.Schueler.Find(idParsed);
            if (found == null) { return Ok(); }

            db.Entry(found).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            try
            {
                db.SaveChanges();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new { found.S_Nr, found.S_Vorname, found.S_Zuname, found.S_Geschl, found.S_Klasse });

        }

        /// <summary>
        /// Reagiert auf PUT Requests mit der URL /api/pupil/id.
        /// Die Daten müssen in der Form raw als application/json übermittelt werden.
        /// Daher ist in Postman unter Body "raw" und "JSON" einzustellen.
        /// Darf nur von Lehrern verwendet werden.
        /// </summary>
        [Authorize(Roles = "Teacher")]
        [HttpPut("{id}")]
        public IActionResult PutFromForm(string id, [FromBody] Schueler schueler)
        {
            if (!long.TryParse(id, out long idParsed)) { return BadRequest(); }

            Schueler found = db.Schueler.Find(idParsed);
            if (found == null) { return Ok(); }

            try
            {
                found.S_Geschl = schueler.S_Geschl;
                // S_Klasse müssen wir nicht setzen, es wird von EF durch die Navigation beim Setzen
                // des EntityState auf Modified gesetzt.
                found.S_KlasseNavigation = db.Klasse.Find(schueler.S_Klasse) ?? throw new InvalidOperationException("Invalid Class");
                found.S_Vorname = schueler.S_Vorname;
                found.S_Zuname = schueler.S_Zuname;
                db.Entry(found).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                db.SaveChanges();
            }
            // Tritt auf, wenn die Klasse nicht gefunden wurde.
            catch (InvalidOperationException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return Ok(new { found.S_Nr, found.S_Vorname, found.S_Zuname, found.S_Geschl, found.S_Klasse });
        }
    }
}