using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestAdministrator.Api.Model;

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

        [HttpGet]
        [ProducesResponseType(200)]
        public IEnumerable<string> Get()
        {
            var records = _context.Test.Count();
            // Ein Array zurückgeben, damit ein gültiges JSON entsteht. Ein Einzelwert ist keine
            // gültige JSON Antwort.
            return new string[] { $"{records} Datensätze gefunden." };
        }
    }
}