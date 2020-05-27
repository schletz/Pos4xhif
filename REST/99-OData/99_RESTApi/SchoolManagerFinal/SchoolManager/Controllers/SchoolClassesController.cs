using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using SchoolManager.Model;
using System.Collections.Generic;
using System.Linq;

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
        [EnableQuery()]
        public IEnumerable<SchoolClasses> Get()
        {
            return _dbContext.SchoolClasses;
        }
    }
}