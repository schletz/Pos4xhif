using Microsoft.AspNetCore.Mvc;
using SPG.AirQuality.Models;
using System.Collections.Generic;

namespace SPG.AirQuality.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MeasurementsController : ControllerBase
    {
        private readonly AirQualityContext _dbContext;

        public MeasurementsController(AirQualityContext dbContext)
        {
            _dbContext = dbContext;
        }

        // URL: https://localhost:5001/odata/Measurements?...

        [HttpGet()]
        public IEnumerable<Measurements> Get()
        {
            var result = _dbContext.Measurements;

            return result;
        }
    }
}
