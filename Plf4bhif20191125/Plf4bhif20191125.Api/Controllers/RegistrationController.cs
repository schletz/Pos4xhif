using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plf4bhif20191125.Dto;
using Plf4bhif20191125.Model;

namespace Plf4bhif20191125.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationContext _db;
        public RegistrationController(RegistrationContext db)
        {
            _db = db;
        }

        [HttpGet("test")]
        public ActionResult<RegistrationDto> GetTestdata()
        {
            return Ok(from r in _db.Registration
                      select new RegistrationDto
                      {
                          Firstname = r.R_Firstname,
                          Lastname = r.R_Lastname
                      });
        }

    }
}