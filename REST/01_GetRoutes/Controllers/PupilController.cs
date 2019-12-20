using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GetRoutes.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GetRoutesDemo.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PupilController : ControllerBase
    {
        private readonly SchuelerDb db;           // using GetRoutes.Model;
        public PupilController()
        {
            db = SchuelerDb.FromMockup();                // Wird später durch Dependency Injection ersetzt.
        }

        /// <summary>
        /// Reagiert auf /api/pupil und gibt eine Liste aller Schüler aus.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IEnumerable<Pupil> GetPupils()
        {
            return db.Pupil;
        }

        /// <summary>
        /// Reagiert z. B. auf /api/pupil/1001 und gibt hier die Daten des Schülers 1001 aus.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Pupil> GetPupilById(int id)
        {
            Pupil p = db.Pupil.SingleOrDefault(p => p.Id == id);
            if (p == null) { return NotFound(); }
            return Ok(p);
        }

        /// <summary>
        /// Reagiert auf /api/count und gibt die Anzahl der Schüler aus.
        /// </summary>
        [HttpGet("/api/count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public int GetPupilCount(int id)
        {
            return db.Pupil?.Count() ?? 0;
        }

        /// <summary>
        /// Reagiert auf /api/pupil/byId?id=1001. Mit FromQuery kann auch eine Anfrage mit dieser
        /// Form ausgewertet werden.
        /// </summary>
        [HttpGet("byId")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Pupil> GetWithQuerystring([FromQuery]int id)
        {
            return GetPupilById(id);
        }
    }
}