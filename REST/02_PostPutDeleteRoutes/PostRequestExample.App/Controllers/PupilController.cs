using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostRequestExample.App.Model;

namespace PostRequestExample.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PupilController : ControllerBase
    {
        private readonly SchuelerDb db = SchuelerDb.FromMockup();
        /// <summary>
        /// Reagiert GET Requests mit der URL api/pupil
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(db.Schueler.Select(s => new { s.Vorname, s.Zuname }));
        }

        /// <summary>
        /// Reagiert GET Requests mit der URL api/pupil/1001
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetId(string id)
        {
            if (!int.TryParse(id, out int idParsed))
            {
                return Ok();            // Alternativ: return BadRequest();
            }
            return Ok(db.Schueler.FirstOrDefault(s => s.Nr == idParsed));
        }
        /// <summary>
        /// Reagiert auf POST Requests mit der URL /api/pupil.
        /// Die POST Daten müssen in der Form form-data oder x-www-form-urlencoded übermittelt werden.
        /// Daher ist in Postman unter Body "form-data" oder x-www-form-urlencoded einzustellen.
        /// Die Parameter müssen wie die Properties von Schueler heißen.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult PostFromForm([FromForm] Schueler schueler)
        {
            db.Schueler.Add(schueler);
            return Ok(schueler);
        }

        /// <summary>
        /// Reagiert auf POST Requests mit der URL /api/pupil/rawdata.
        /// Die Daten müssen in der Form raw als application/json übermittelt werden.
        /// Daher ist in Postman unter Body "raw" und "JSON" einzustellen.
        /// Daten:
        /// {
        ///	"Nr": 1,
        ///	"Klasse": "5AHIF",
        ///	"Zuname": "Mustermann",
        ///	"Vorname": "Max",
        ///	"Geschl": "m"
        /// }
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost("rawdata")]
        public IActionResult PostFromBody([FromBody] Schueler schueler)
        {
            db.Schueler.Add(schueler);
            return Ok(schueler);
        }

        /// <summary>
        /// Reagiert auf DELETE Requests mit der URL /api/pupil.
        /// Die Daten müssen in der Form form-data oder x-www-form-urlencoded übermittelt werden.
        /// Daher ist in Postman unter Body "form-data" oder x-www-form-urlencoded einzustellen.
        /// Die Parameter müssen wie die Properties von Schueler heißen.
        /// </summary>
        /// <param name="schueler"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult DeleteFromForm([FromForm] Schueler schueler)
        {
            Schueler found = db.Schueler.FirstOrDefault(s => s.Nr == schueler.Nr);
            if (found != null)
            {
                db.Schueler.Remove(found);
                return Ok(schueler);
            }
            else
            {
                return Ok();
            }
        }

        /// <summary>
        /// Reagiert auf PUT Requests mit der URL /api/pupil.
        /// Die Daten müssen in der Form form-data oder x-www-form-urlencoded übermittelt werden.
        /// Daher ist in Postman unter Body "form-data" oder x-www-form-urlencoded einzustellen.
        /// Die Parameter müssen wie die Properties von Schueler heißen.
        /// </summary>
        /// <param name="schueler"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult PutFromForm([FromForm] Schueler schueler)
        {
            Schueler found = db.Schueler.FirstOrDefault(s => s.Nr == schueler.Nr);
            if (found != null)
            {
                db.Schueler.Remove(found);
                db.Schueler.Add(schueler);
                return Ok(schueler);
            }
            else
            {
                return Ok();
            }
        }
    }
}