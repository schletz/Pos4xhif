using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestAdministrator.Api.Model;
using TestAdministrator.Dto;

namespace TestAdministrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly TestsContext _context;
        private readonly string _authTeacherId;

        /// <summary>
        /// Konstruktor. Liest den angemeldeten User aus dem Token und ermittelt die Lehrer-ID
        /// des angemeldeten Benutzers (wenn es eine gibt)
        /// </summary>
        /// <param name="contextAccessor">
        /// Hinweis: Braucht services.AddHttpContextAccessor() in ConfigureServices
        /// </param>
        /// <param name="context"></param>
        public TestController(IHttpContextAccessor contextAccessor, TestsContext context)
        {
            _context = context;
            string username = contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            // Die Lehrer-ID des anfragenden Users herausfinden.
            _authTeacherId = _context.Teacher.SingleOrDefault(t => t.T_Account == username)?.T_ID;
        }


        /// <summary>
        /// Reagiert auf /api/testsbyuser/(username)
        /// Liefert Informationen zu den eingetragenen Tests, die für den User interessant sind.
        /// Eine Route ohne Username, die mit dem angemeldeten User arbeitet, ist zwar möglich, 
        /// wäre aber nicht stateless.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/testsbyuser/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<IEnumerable<TestDto>> Get(string userId)
        {
            // Wer fragt an?
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            // Jeder User darf nur Infos über seine Tests abfragen. Ist der abgefragte User nicht
            // der angemeldete User, so wird 403 Forbidden geliefert.
            if (userId != username)
            { return Forbid(); }
            IEnumerable<TestDto> result = Enumerable.Empty<TestDto>();

            // Ist es ein Lehrer, so suchen wir nach allen Tests, wo dieser Lehrer prüft.
            if (_context.Teacher.Any(t => t.T_Account == username))
            {
                result = from t in _context.Test
                         where t.TE_TeacherNavigation.T_Account == username
                         select new TestDto
                         {
                             TestId = t.TE_ID,
                             Schoolclass = t.TE_Class,
                             Teacher = t.TE_Teacher,
                             Subject = t.TE_Subject,
                             DateFrom = t.TE_Date + t.TE_LessonNavigation.P_From,
                             Lesson = t.TE_Lesson
                         };
            }
            else
            {
                // Bei einem Schüler suchen wir nach seiner Klasse und den Tests dieser Klasse.
                string schoolclass = _context.Pupil.FirstOrDefault(p => username == p.P_Account)?.P_Class ?? "";
                result = from t in _context.Test
                         where t.TE_Class == schoolclass
                         select new TestDto
                         {
                             TestId = t.TE_ID,
                             Schoolclass = t.TE_Class,
                             Teacher = t.TE_Teacher,
                             Subject = t.TE_Subject,
                             DateFrom = t.TE_Date + t.TE_LessonNavigation.P_From,
                             Lesson = t.TE_Lesson
                         };
            }
            return Ok(result);
        }

        /// <summary>
        /// Liefert die Unterrichtsstunden (Kombination aus Klasse und Fach), die der Lehrer
        /// lt. Stundenplan unterrichtet.
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        [HttpGet("/api/lessons/{teacherId}")]
        [Authorize(Roles = "teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<LessonDto> GetLessons(string teacherId)
        {
            if (teacherId != _authTeacherId) { return Forbid(); }

            var subjects = (from l in _context.Lesson
                            where l.L_Teacher == teacherId
                            group l by new { l.L_Class, l.L_Subject } into g
                            select new LessonDto
                            {
                                Class = g.Key.L_Class,
                                Subject = g.Key.L_Subject
                            });
            return Ok(subjects);
        }

        /// <summary>
        /// Reagiert auf POST /api/test
        /// Trägt einen Test eines Lehrers ein.
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="testinfo"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TestDto> Post([FromBody] TestDto testinfo)
        {
            // Der User darf nur für sich Tests eintragen.
            if (testinfo.Teacher != _authTeacherId) { return Forbid(); }
            try
            {
                Test newTest = new Test
                {
                    TE_ClassNavigation = _context.Schoolclass.Find(testinfo.Schoolclass),
                    TE_LessonNavigation = _context.Period.Find(testinfo.Lesson),
                    TE_Date = testinfo.DateFrom,
                    TE_Subject = testinfo.Subject,
                    TE_TeacherNavigation = _context.Teacher.Find(testinfo.Teacher)
                };
                _context.Test.Add(newTest);
                _context.SaveChanges();
                // Das Objekt mit den Daten aus der DB zurückgeben, damit es sicher gleich mit
                // den eingetragenen Werten ist.
                return Ok(new TestDto
                {
                    DateFrom = newTest.TE_Date,
                    Lesson = newTest.TE_Lesson,
                    Schoolclass = newTest.TE_Class,
                    Subject = newTest.TE_Subject,
                    Teacher = newTest.TE_Teacher,
                    TestId = newTest.TE_ID
                });
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Reagiert auf PUT /api/test/(testId)
        /// Ändert einen eingetragenen Test in der Datenbank. Die Lehrer ID darf natürlich nicht
        /// geändert werden.
        /// </summary>
        /// <param name="teacherId"></param>
        /// <param name="testinfo"></param>
        /// <returns></returns>
        [HttpPut("{testId}")]
        [Authorize(Roles = "teacher")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TestDto> Put(long testId, [FromBody] TestDto testinfo)
        {
            // Natürlich darf ein Lehrer - auch wenn er korrekt angemeldet ist - nur Test-IDs von
            // seinen Tests ändern.
            Test found = _context.Test
                .Where(t => t.TE_Teacher == _authTeacherId && t.TE_ID == testId).SingleOrDefault();
            if (found == null) { return NotFound(); }
            try
            {
                found.TE_ClassNavigation = _context.Schoolclass.Find(testinfo.Schoolclass);
                found.TE_LessonNavigation = _context.Period.Find(testinfo.Lesson);
                found.TE_Date = testinfo.DateFrom;
                found.TE_Subject = testinfo.Subject;
                _context.SaveChanges();
                // Das Objekt mit den Daten aus der DB zurückgeben, damit es sicher gleich mit
                // den eingetragenen Werten ist.
                return Ok(new TestDto
                {
                    DateFrom = found.TE_Date,
                    Lesson = found.TE_Lesson,
                    Schoolclass = found.TE_Class,
                    Subject = found.TE_Subject,
                    Teacher = found.TE_Teacher,
                    TestId = found.TE_ID
                });
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>
        /// Reagiert auf DELETE /api/test/(testID)
        /// Löscht den eingetragenen Test aus der Datenbank.
        /// </summary>
        /// <param name="testId"></param>
        /// <returns></returns>
        [HttpDelete("{testId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult Delete(long testId)
        {
            // Natürlich darf ein Lehrer - auch wenn er korrekt angemeldet ist - nur Test-IDs von
            // seinen Tests ändern.
            Test found = _context.Test
                .Where(t => t.TE_Teacher == _authTeacherId && t.TE_ID == testId).SingleOrDefault();
            if (found == null) { return NotFound(); }
            try
            {
                _context.Test.Remove(found);
                _context.SaveChanges();
                return Ok();
            }
            catch (DbUpdateException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
