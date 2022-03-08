using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExamManager.Webapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JsonTestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok(new {Message = "Hello World!"});
    }
}
