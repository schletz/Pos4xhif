using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MasterDetailDemo.Api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MasterDetailDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PupilsController : ControllerBase
    {
        private readonly TestsContext context;
        public PupilsController (TestsContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(context.Pupil);
        }
    }
}