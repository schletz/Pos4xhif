using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManager.Model;

namespace SchoolManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolClassesController : ControllerBase
    {
        private readonly SchoolContext _dbContext;

        public SchoolClassesController(SchoolContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet()]
        public IEnumerable<SchoolClasses> Get()
        {
            return _dbContext.SchoolClasses;
        }
    }
}