using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAdministrator.Api.Model;
using TestAdministrator.Dto;

namespace TestAdministrator.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly TestsContext _context;

        public DashboardController(TestsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Reagiert auf /api/dashboard/(username)
        /// Liefert Informationen zu den eingetragenen Tests, die für den User interessant sind.
        /// Eine Route ohne Username, die mit dem angemeldeten User arbeitet, ist zwar möglich, 
        /// wäre aber nicht stateless.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public ActionResult<IEnumerable<TestinfoDto>> Get(string userId)
        {
            // Wer fragt an?
            string username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            // Jeder User darf nur Infos über seine Tests abfragen. Ist der abgefragte User nicht
            // der angemeldete User, so wird 403 Forbidden geliefert.
            if (userId != username)
            { return Forbid(); }
            IEnumerable<TestinfoDto> result = Enumerable.Empty<TestinfoDto>();

            // Ist es ein Lehrer, so suchen wir nach allen Tests, wo dieser Lehrer prüft.
            if (_context.Teacher.Any(t => t.T_Account == username))
            {
                result = from t in _context.Test
                         where t.TE_TeacherNavigation.T_Account == username
                         select new TestinfoDto
                         {
                             Schoolclass = t.TE_Class,
                             Teacher = t.TE_Teacher,
                             Subject = t.TE_Subject,
                             DateFrom = t.TE_Date + t.TE_LessonNavigation.P_From
                         };
            }
            else
            {
                // Bei einem Schüler suchen wir nach seiner Klasse und den Tests dieser Klasse.
                string schoolclass = _context.Pupil.FirstOrDefault(p => username == p.P_Account)?.P_Class ?? "";
                result = from t in _context.Test
                         where t.TE_Class == schoolclass
                         select new TestinfoDto
                         {
                             Schoolclass = t.TE_Class,
                             Teacher = t.TE_Teacher,
                             Subject = t.TE_Subject,
                             DateFrom = t.TE_Date + t.TE_LessonNavigation.P_From
                         };
            }
            return Ok(result);
        }
    }
}