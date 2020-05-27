using Microsoft.AspNetCore.Mvc;
using SPG.AirQuality.Models;
using System.Collections.Generic;

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
        public IEnumerable<Stations> Get()
        {
            return _dbContext.Stations;
        }
    }
}
