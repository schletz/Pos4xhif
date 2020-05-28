using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SPG.AirQuality.Models;

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
        [EnableQuery()]
        public IQueryable<Measurements> Get()
        {
            var result = _dbContext.Measurements;

            return result;
        }
    }
}
