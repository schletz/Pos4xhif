using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPG.AirQuality.Models;
using System.Collections.Generic;
using System.Linq;

namespace SPG.AirQuality.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController : ControllerBase
    {
        private readonly AirQualityContext _dbContext;

        public StationsController(AirQualityContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet()]
        [EnableQuery()]
        public IEnumerable<Stations> Get()
        {
            return _dbContext.Stations;
        }
    }
}
