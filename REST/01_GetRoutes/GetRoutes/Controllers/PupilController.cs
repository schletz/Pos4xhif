using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using GetRoutes.Model;

namespace GetRoutes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PupilController : ControllerBase
    {
        private readonly SchuelerDb db = SchuelerDb.FromMockup();
        /// <summary>
        /// Reagiert auf api/Pupil
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(db.Schueler.Select(s => new { s.Vorname, s.Zuname }));
        }

        /// <summary>
        /// Reagiert auf api/Pupil/1001.
        /// Die Route ist relativ, d. h. api/pupil wird von der Controller Route
        /// übernommen.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetById(string id)
        {
            int parsedId = -1;

            if (int.TryParse(id, out parsedId))
            {
                return Ok(db
                    .Schueler
                    .Where(s => s.Nr == parsedId)
                    .Select(s => new { s.Vorname, s.Zuname }));
            }
            else
                // HTTP 404, wenn der Schüler nicht gefunden wurde. Ein leeres Ergerebnis ist
                // allerdings sinnvoller. Es soll nur zeigen, wie 404 zurückgegeben wird.
                return NotFound();
        }

        /// <summary>
        /// Reagiert auf /api/pupil/byId?id=1001. Mit FromQuery kann auch eine Anfrage mit dieser
        /// Form ausgewertet werden.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("byId")]
        public IActionResult GetWithQuerystring([FromQuery]string id)
        {
            int parsedId = -1;

            if (int.TryParse(id, out parsedId))
            {
                return Ok(db
                    .Schueler
                    .Where(s => s.Nr == parsedId)
                    .Select(s => new { s.Vorname, s.Zuname }));
            }
            else
                // HTTP 404, wenn der Schüler nicht gefunden wurde. Ein leeres Ergerebnis ist
                // allerdings sinnvoller. Es soll nur zeigen, wie 404 zurückgegeben wird.
                return NotFound();
        }

        [HttpGet("/api/count")]
        public IActionResult GetPupilCount()
        {
            return Ok(db.Schueler.Count());
        }
    }
}