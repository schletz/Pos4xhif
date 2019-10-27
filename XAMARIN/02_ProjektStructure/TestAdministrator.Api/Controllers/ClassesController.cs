using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAdministrator.Api.Model;
using TestAdministrator.Dto;

namespace TestAdministrator.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly TestsContext _context;
        public ClassesController(TestsContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// GET api/classes: Liefert alle Klassen als JSON Array. 
        /// Braucht ein Login, außer [AllowAnonymous]
        /// wird gesetzt.
        /// </summary>
        /// <returns>
        /// HTTP 200: JSON Array mit allen Klassen der Datenbank.
        /// HTTP 401: Nicht authentifiziert.
        /// HTTP 500: Datenbank- oder Serverfehler.        /// </returns>
        //[AllowAnonymous]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var allClasses = from c in _context.Schoolclass
                                 select new SchoolclassDto
                                 {
                                     Id = c.C_ID,
                                     Department = c.C_Department,
                                     ClassTeacher = c.C_ClassTeacher
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
        /// GET /api/classes/(klassenname): Liefert Details zu einer Klasse als JSON Object.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        /// HTTP 200: JSON Object mit den Klassendetails oder leer be nicht gefundener Klasse.
        /// HTTP 401: Nicht authentifiziert.
        /// HTTP 403: Der User hat nicht die Rolle Teacher.
        /// HTTP 500: Datenbank- oder Serverfehler.
        /// </returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Teacher")]
        public IActionResult GetId(string id)
        {
            try
            {
                Schoolclass found = _context.Schoolclass.Find(id);
                if (found == null) return Ok();
                return Ok(new SchoolclassDto
                {
                    Id = found.C_ID,
                    Department = found.C_Department,
                    ClassTeacher = found.C_ClassTeacher
                });
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
