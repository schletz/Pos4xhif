// *************************************************************************************************
// REST CONTROLLER FÜR SCHÜLERDATEN
// *************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PostRoutesDemo.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PostRoutesDemo.Controller
{
    /// <summary>
    /// Reagiert auf /api/pupil
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PupilController : ControllerBase
    {
        // Der Controller wird bei jedem Request neu instanzierst. Damit unsere Änderungen erhalten
        // bleiben verwenden wir static.
        private static readonly SchuelerDb db = SchuelerDb.FromMockup();    // using GetRoutes.Model

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
        /// Reagiert auf /api/pupil/{id} und gibt hier die Daten des Schülers mit der id aus.
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
        /// Reagiert auf POST /api/pupil
        /// Fügt den übergebenen Schüler in die Liste ein und weist eine neue ID zu.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Pupil> Post(Pupil pupil)
        {
            try
            {
                int newId = db.Pupil.Max(p => p.Id) + 1;          // Simuliert eine Autoincrement Id.
                pupil.Id = newId;
                db.Pupil.Add(pupil);
                return Ok(pupil);                 // Den Schüler mit der generierten Id zurückgeben.
            }
            // Kann der Schüler nicht angelegt werden (z. B. Verletzung von Contraints in der Db),
            // wird 400 geliefert.
            catch
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Reagiert auf POST /api/pupil/fromForm
        /// Fügt den übergebenen Schüler in die Liste ein und weist eine neue ID zu.
        /// Die POST Daten müssen in der Form form-data oder x-www-form-urlencoded übermittelt werden.
        /// Daher ist in Postman unter Body "form-data" oder x-www-form-urlencoded einzustellen. 
        /// </summary>
        [HttpPost("fromForm")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Pupil> PostFromForm([FromForm] Pupil pupil) => Post(pupil);


        /// <summary>
        /// Reagiert auf PUT /api/pupil/{id}
        /// Aktualisiert die Properties eines existierenden Schülers auf die übergebenen Werte.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Pupil> Put(int id, Pupil pupil)
        {
            try
            {
                Pupil found = db.Pupil.SingleOrDefault(p => p.Id == id);
                if (found == null) { return NotFound(); }
                // Simuliert das Aktualisieren des Datensatzes in der Db. Dabei darf der Primärschlüssel
                // (die Id) nicht geändert werden. Ob alle anderen Properties geändert werden dürfen
                // ist natürlich Sache der Programmlogik.
                found.Class = pupil.Class;
                found.Lastname = pupil.Lastname;
                found.Firstname = pupil.Firstname;
                found.Gender = pupil.Gender;
                return Ok(found);
            }
            // Kann der Schüler nicht aktualisiert werden (z. B. Verletzung von Contraints in der Db),
            // wird 400 geliefert.
            catch
            {
                return BadRequest();
            }
        }


        /// <summary>
        /// Reagiert auf DELETE /api/pupil/{id}
        /// Löscht einen Schüler aus der Collection.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public StatusCodeResult Delete(int id)
        {
            try
            {
                Pupil found = db.Pupil.SingleOrDefault(p => p.Id == id);
                if (found == null) { return NotFound(); }
                db.Pupil.Remove(found);
                return Ok();
            }
            // Wenn der Schüler als Fremdschlüssel verwendet wird, kann er z. B. in einer Db nicht
            // gelöscht werden.
            catch
            {
                return BadRequest();
            }
        }
    }
}