using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCore.Model;

namespace EntityFrameworkCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PupilController : ControllerBase
    {
        private readonly TestsContext _context;

        /// <summary>
        /// Speichert den über Dependency Injection übergebenen DB Context.
        /// Dabei ist in Startup.cs in ConfigureServices() die Zeile
        ///     services.AddDbContext<Model.TestsContext>();
        /// einzufügen.
        /// </summary>
        public PupilController(TestsContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Ragiert auf GET /api/pupil
        /// Liefert alle Schüler in der Pupil Tabelle als JSON.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPupil()
        {
            // Damit wir das Entity nicht 1:1 serialisieren, wählen wir die Inhalte aus.
            var result = await (from p in _context.Pupil
                                select new
                                {
                                    Id = p.P_ID,
                                    Firstname = p.P_Firstname,
                                    Lastname = p.P_Lastname,
                                    Class = p.P_Class
                                }).ToListAsync();
            return Ok(result);
        }

        /// <summary>
        /// Reagiert auf GET /api/pupil/{id}
        /// Liefert ein Eintrag eines Schülers in der Pupil Tabelle als JSON.
        /// Dabei werden die JSON Annotations (JsonIgnore, JsonPropertyName) in der Modelklasse
        /// Pupil berücksichtigt.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Pupil>> GetPupil(long id)
        {
            // Navigationen in der Entity Klasse Pupil können über 
            // [System.Text.Json.Serialization.JsonIgnore] ausgeschlossen werden.
            Pupil pupil = await _context.Pupil.FindAsync(id);

            if (pupil == null) { return NotFound(); }
            return pupil;
        }

        /// <summary>
        /// Reagiert auf POST /api/pupil
        /// Schreibt einen neuen Schüler in die Datenbank.
        /// </summary>
        /// <param name="pupil">Schülerdaten als JSON im Request Body.</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<Pupil>> PostPupil(Pupil pupil)
        {
            // Felder mit [Required] in der Modelklasse werden automatisch geprüft. Wurden sie
            // nicht übermittelt, so wird BadRequest() erzeugt. Darum müssen wir uns nicht
            // kümmern.
            _context.Pupil.Add(pupil);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Hier landet man, wenn Contraints fehlschlagen. Möchte man diese feiner
                // abprüfen, so müssen sie im Programmcode vorab geprüft werden.
                return Conflict();
            }

            // Liefert den Inhalt des Requests GET /api/pupil/{id} und HTTP 201 (Created)
            return CreatedAtAction(nameof(GetPupil), new { id = pupil.P_ID }, pupil);
        }

        /// <summary>
        /// Reagiert auf PUT /api/pupil/{id}
        /// <param name="pupil">Schülerdaten als JSON im Request Body.</param>
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PutPupil(long id, Pupil pupil)
        {
            // Request weicht vom Inhalt ab?
            if (id != pupil.P_ID) { return BadRequest(); }
            _context.Entry(pupil).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Es wurde versucht, einen Schüler zu aktualisieren, den es nicht gibt.
                if (!_context.Pupil.Any(e => e.P_ID == id)) { return NotFound(); }
                else { throw; }
            }
            catch (DbUpdateException)
            {
                // Eine Kollision beim Aktualisieren (Femdschlüssel, ...) trat auf.
                return Conflict();
            }
            // Unbehandelte Fehler führen automatisch zu HTTP 500.

            return NoContent();
        }


        /// <summary>
        /// Reagiert auf DELETE /api/pupil/{id}
        /// Löscht den Schüler mit der angegebenen ID aus der Pupil Tabelle.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Pupil>> DeletePupil(long id)
        {
            Pupil pupil = await _context.Pupil.FindAsync(id);
            if (pupil == null) { return NotFound(); }
            _context.Pupil.Remove(pupil);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Es kann vorkommen, dass die Daten als FK verwendet werden.
                return Conflict();
            }
            return NoContent();
        }
    }
}
