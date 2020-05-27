using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using SchoolManager.Model;
using System.Linq;

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
        [EnableQuery()]
        public IQueryable<Pupils> Get()
        {
            return _dbContext.Pupils;
        }
    }
}