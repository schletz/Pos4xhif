using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAdministrator.Api.Model;
using TestAdministrator.Dto;

namespace TestAdministrator.Api.Controllers
{
    /// <summary>
    /// Liefert Daten zu den Schulklassen.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly TestsContext _context;
        /// <summary>
        /// Konstruktor. Setzt den DB Context.
        /// </summary>
        /// <param name="context">Der über services.AddDbContext() gesetzte Context.</param>
        public ClassesController(TestsContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// GET api/classes
        /// Liefert alle Klassen als JSON Array. Braucht ein Login, außer 
        /// [AllowAnonymous] wird gesetzt.
        /// </summary>
        /// <returns>
        /// HTTP 200: JSON Array mit allen Klassen der Datenbank.
        /// HTTP 401: Nicht authentifiziert.
        /// HTTP 500: Datenbank- oder Serverfehler.        
        /// </returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<SchoolclassDto>> Get()
        {
            try
            {
                var allClasses = from c in _context.Schoolclass
                                 select new SchoolclassDto
                                 {
                                     Id = c.C_ID,
                                     Department = c.C_Department,
                                     ClassTeacher = c.C_ClassTeacher,
                                     StudentCount = c.Pupil.Count()
                                 };
                return Ok(allClasses);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "DB Error" });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = e.Message, Details = e.InnerException?.Message });
            }
        }

        /// <summary>
        /// GET /api/classes/(klassenname): 
        /// Liefert Details zu einer Klasse als JSON Object.
        /// </summary>
        /// <param name="id">Eindeutiger Name, nach dem in der Datenbank gesucht wird.</param>
        /// <returns>
        /// HTTP 200: JSON Object mit den Klassendetails oder leer be nicht gefundener Klasse.
        /// HTTP 401: Nicht authentifiziert.
        /// HTTP 403: Nicht autorisiert, der User hat nicht die Rolle Teacher.
        /// HTTP 500: Datenbank- oder Serverfehler.
        /// </returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<SchoolclassDto> GetId(string id)
        {
            try
            {
                // Mit Find() bekommen wir keine Navigation Properties. Daher filtern wir mit Where.
                // Durch FirstOrDefault wird ein JSON Object und kein Array geliefert.
                var result = (from s in _context.Schoolclass
                              where s.C_ID.ToUpper() == id.ToUpper()
                              select new SchoolclassDto
                              {
                                  Id = s.C_ID,
                                  Department = s.C_Department,
                                  ClassTeacher = s.C_ClassTeacher,
                                  StudentCount = s.Pupil.Count(),
                                  // Alle Lehrer, die diese Klasse unterrichten, liefern.
                                  // Da LINQ to SQL bei Unterabfragen nicht alles generieren kann,
                                  // laden wir die Zwischenergebnisse mit ToList() in den Speicher.
                                  // Das letzte ToList() ist für den Scope wichtig.
                                  Teachers = (from t in _context.GetClassTeachers(id).ToList()
                                              select new TeacherDto
                                              {
                                                  Id = t.T_ID,
                                                  Firstname = t.T_Firstname,
                                                  Lastname = t.T_Lastname,
                                                  Email = t.T_Email
                                              }).ToList()
                              }).FirstOrDefault();
                return Ok(result);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "DB Error" });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = e.Message, Details = e.InnerException?.Message });
            }
        }
    }
}
