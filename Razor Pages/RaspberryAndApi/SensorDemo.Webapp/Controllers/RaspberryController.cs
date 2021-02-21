using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using SensorDemo.Webapp.Dto;
using SensorDemo.Webapp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SensorDemo.Webapp.Controllers
{
    /// <summary>
    /// Controller für die Requests des Raspberry PI. Erreichbar unter
    ///     api/raspberry
    /// Muss bei einer Razor Pages app mit
    ///     endpoints.MapControllers();
    /// bei app.UseEndpoints() in ConfigureServices registriert werden.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(CheckApiKeyFilterAttribute))]
    public class RaspberryController : ControllerBase
    {
        private readonly HearbeatService _hearbeatService;

        public RaspberryController(HearbeatService hearbeatService)
        {
            _hearbeatService = hearbeatService;
        }

        /// <summary>
        /// Reagiert auf POST /api/raspberry/sensordata und liest das übergebene JSON in den Typ
        /// SensordataDto.
        /// TODO: Persistieren der Daten.
        /// </summary>
        [HttpPost("sensordata")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult SaveSensordata([FromBody] SensordataDto data)
        {
            Console.WriteLine($"Received temperature {data.Temperature}°  with timestamp {data.Timestamp}.");
            return NoContent();
        }

        /// <summary>
        /// Reagiert auf POST /api/raspberry/heartbeat und setzt den letzten empfangenen Hearbeat
        /// im Service.
        /// </summary>
        [HttpPost("heartbeat")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult SetHeartbeat([FromBody] double timestamp)
        {
            Console.WriteLine($"Received hearbeat with timestamp {timestamp}.");
            _hearbeatService.SetHeartbeat(timestamp);
            return NoContent();
        }

    }


    /// <summary>
    /// Filterklasse zum Prüfen des API Key Headers
    /// TODO: Hinzufügen eines base64 codierten Secrets z. B. von https://generate.plus/en/base64
    /// als Eintrag
    ///       "Secret": "....",
    /// in der appsettings.json. 128 Bytes (1024 Bits) sind ein angemessener Wert.
    /// TODO: Registrieren in ConfigureServices mit
    ///       services.AddTransient(provider => new Controllers.CheckApiKeyFilterAttribute(
    ///           secret: Configuration["Secret"]));
    /// </summary>
    public class CheckApiKeyFilterAttribute : ActionFilterAttribute
    {
        // Speichert die letzten verwendeten Token, damit jeder Token nur 1x verwendet werden kann.
        // Achtung: Shared Memory, Zugriff muss thread safe sein!
        private static double[] _usedTokens = new double[1024];
        private static int _usedTokensPos = 0;

        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly TimeSpan maxAge = TimeSpan.FromMinutes(5);

        private readonly byte[] _secret;
        public CheckApiKeyFilterAttribute(string secret)
        {
            _secret = Convert.FromBase64String(secret);
        }
        /// <summary>
        /// Setzt die Response auf HTTP Forbidden (403), falls der übermittelte Key ungültig ist.
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Wird überhaupt ein X-API-Key Header im Request übermittelt?
            if (!context.HttpContext.Request.Headers.TryGetValue("X-API-Key", out var key))
            {
                context.Result = new StatusCodeResult(403); return;
            }
            var parts = key[0].Split(":");
            // Hat der Key den Aufbau payload:hash?
            if (parts.Length != 2)
            {
                context.Result = new StatusCodeResult(403); return;
            }
            // Stimmt der Hashwert, wurde also dieser payload mit dem definierten Secret gehashed?
            var dataBytes = System.Text.Encoding.UTF8.GetBytes(parts[0]);
            var hash = new System.Security.Cryptography.HMACSHA256(_secret).ComputeHash(dataBytes);
            if (Convert.ToBase64String(hash) != parts[1])
            {
                context.Result = new StatusCodeResult(403); return;
            }
            // Ist der Payload ein gültiger Unix Timestamp (double Wert)?
            if (!double.TryParse(parts[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double timestamp))
            {
                context.Result = new StatusCodeResult(403); return;
            }
            // Der Timestamp darf maximal maxAge alt sein.
            double currentTimestamp = (DateTime.UtcNow - _epoch).TotalSeconds;
            if (Math.Abs(currentTimestamp - timestamp) > maxAge.TotalSeconds)
            {
                context.Result = new StatusCodeResult(403); return;
            }
            // Wurde der Token schon verwendet?
            if (_usedTokens.Contains(timestamp))
            {
                context.Result = new StatusCodeResult(403); return;
            }
            // Den verwendeten Timestamp thread safe ins Array speichern.
            lock (_usedTokens)
            {
                _usedTokens[_usedTokensPos] = timestamp;
                _usedTokensPos = (_usedTokensPos + 1) % _usedTokens.Length;
            }
        }
    }

}
