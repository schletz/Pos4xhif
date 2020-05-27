using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManager.Model;

namespace SchoolManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PupilsController : ControllerBase
    {
        private readonly SchoolContext _dbContext;

        public PupilsController(SchoolContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet()]
        public IEnumerable<Pupils> Get()
        {
            return _dbContext.Pupils;
        }
    }
}