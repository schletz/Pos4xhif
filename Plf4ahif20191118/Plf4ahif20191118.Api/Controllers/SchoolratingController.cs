using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plf4ahif20191118.Model;
using Plf4ahif20191118.Dto;

namespace Plf4ahif20191118.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolratingController : ControllerBase
    {
        private readonly RatingContext _db;
        public SchoolratingController(RatingContext context)
        {
            _db = context;
        }
        /// <summary>
        /// Testroute zum Prüfen ob alles funktioniert.
        /// </summary>
        /// <returns></returns>
        [HttpGet("test")]
        public ActionResult<SchoolDto> GetSchoolTest()
        {
            return Ok(from s in _db.School
                      select new SchoolDto
                      {
                          Nr = s.S_Nr,
                          Name = s.S_Name
                      });
        }

    }
}
