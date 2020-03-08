using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public TestController(TestsContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Reagiert auf /api/test/(username)
        /// Liefert Informationen zu den eingetragenen Tests, die für den User interessant sind.
        /// Eine Route ohne Username, die mit dem angemeldeten User arbeitet, ist zwar möglich, 
        /// wäre aber nicht stateless.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{userId}")]
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

        [HttpGet("/api/lessons/{userId}")]
        [Authorize(Roles = "teacher")]
        public ActionResult<LessonDto> GetLessons(string userId)
        {
            // Wer fragt an?
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            // Jeder User darf nur Infos über seine Tests abfragen. Ist der abgefragte User nicht
            // der angemeldete User, so wird 403 Forbidden geliefert.
            if (userId != username)
            { return Forbid(); }

            string teacherId = _context.Teacher.SingleOrDefault(t => t.T_Account == userId)?.T_ID;

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

        // POST: api/Test
        [HttpPost("{userId}")]
        [Authorize(Roles = "teacher")]
        public ActionResult<TestDto> Post(string userId, [FromBody] TestDto testinfo)
        {
            // Wer fragt an?
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            // Jeder User darf nur Infos über seine Tests abfragen. Ist der abgefragte User nicht
            // der angemeldete User, so wird 403 Forbidden geliefert.
            if (userId != username)
            { return Forbid(); }

            string teacherId = _context.Teacher.SingleOrDefault(t => t.T_Account == userId)?.T_ID;
            try
            {
                Test newTest = new Test
                {
                    TE_ClassNavigation = _context.Schoolclass.Find(testinfo.Schoolclass),
                    TE_LessonNavigation = _context.Period.Find(testinfo.Lesson),
                    TE_Date = testinfo.DateFrom,
                    TE_Subject = testinfo.Subject,
                    TE_TeacherNavigation = _context.Teacher.Find(teacherId)
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
            catch
            {
                return BadRequest();
            }
        }


        // PUT: api/Test
        [HttpPut("{userId}")]
        [Authorize(Roles = "teacher")]
        public ActionResult<TestDto> Put(string userId, [FromBody] TestDto testinfo)
        {
            // Wer fragt an?
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            // Jeder User darf nur Infos über seine Tests abfragen. Ist der abgefragte User nicht
            // der angemeldete User, so wird 403 Forbidden geliefert.
            if (userId != username)
            { return Forbid(); }

            Test found = _context.Test.Find(testinfo.TestId);
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
            catch
            {
                return BadRequest();
            }
        }


        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public ActionResult Delete(long id)
        {
            Test found = _context.Test.Find(id);
            if (found == null) { return NotFound(); }

            _context.Test.Remove(found);
            _context.SaveChanges();
            return Ok();
        }
    }
}
