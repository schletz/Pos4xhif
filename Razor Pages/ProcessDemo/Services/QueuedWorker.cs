using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProcessDemo.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ProcessDemo.Services
{
    /// <summary>
    /// Service für eine Jobwarteschlange. Es muss in der Datei Program.cs 
    /// mit den notwendigen Argumenten registriert werden:
    /// 
    /// builder.Services.AddSingleton<QueuedWorker>(provider=>
    ///     new QueuedWorker(
    ///            provider.GetRequiredService<IServiceScopeFactory>(),
    ///            provider.GetRequiredService<ILogger<QueuedWorker>>(),
    ///            maxQueueLength: 10,
    ///            maxProcesses: 2,
    ///            timeout: 30000
    /// ));
    /// </summary>
    public class QueuedWorker
    {
        // Record als DTO Klasse für die Parameter eines Jobs. Dies ist natürlich für
        // andere Problemstellungen anzupassen.
        public record Jobinfo(string Username, string Server);

        // Maximale Länge der Warteschlange. Wird diese überschritten, kann der User keinen
        // neuen Job buchen.
        private readonly int _maxQueueLength;
        // So viele Prozesse werden gleichzeitig gestartet. Tipp: Mit
        // Environment.ProcessorCount  kann die CPU Anzahl gelesen werden.
        private readonly int _maxProcesses;
        // Nach dieser Zeitspanne in ms wird dem Prozess ein Kill gesendet.
        private readonly int _timeout;
        // Aktuelle Länge der Warteschlange.
        private int _queueLength = 0;
        private SemaphoreSlim _semaphore;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<QueuedWorker> _logger;

        public QueuedWorker(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<QueuedWorker> logger,
            int maxQueueLength, int maxProcesses, int timeout)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _maxQueueLength = maxQueueLength;
            _maxProcesses = maxProcesses;
            _timeout = timeout;
            _semaphore = new SemaphoreSlim(_maxProcesses, _maxProcesses);
        }

        public (bool success, string message) TryAddJob(Jobinfo jobinfo)
        {
            // Wir müssen thread safe arbeiten, die Methode wird mehrmals aufgerufen.
            // Wenn z. B. in kurzer Zeit die Methode 1000 mal aufgerufen wird, dürfen
            // wird nicht erst später prüfen, ob die Queue zu lange ist. Dann wären
            // schon tausende Jobs eingetragen.
            Interlocked.Increment(ref _queueLength);
            if (_queueLength > _maxQueueLength)
            {
                Interlocked.Decrement(ref _queueLength);
                return (false, "Queue is full");
            }
            // Wir warten nicht auf den Prozess, sonst würde der Controller auch auf
            // die Beendigung warten. Deswegen verwenden wir den Discard Operator _
            // anstatt await.
            _ = EnqueueJob(jobinfo)
                .ContinueWith(task => Interlocked.Decrement(ref _queueLength));
            return (true, string.Empty);
        }

        /// <summary>
        /// Holt sich einen Job von der Warteschlange. Dabei wird darauf geachtet,
        /// dass nicht mehr Jobs als in _maxProcesses gleichtzeitig verarbeitet werden.
        /// </summary>
        private async Task EnqueueJob(Jobinfo jobinfo)
        {
            await _semaphore.WaitAsync();
            try
            {
                await StartJob(jobinfo);
            }
            catch (Exception e)
            {
                WriteJobFailedInfo(jobinfo, DateTime.UtcNow, null, e.InnerException?.Message ?? e.Message);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        /// <summary>
        /// Startet den Prozess, der einen Job in der Warteschlange abarbeitet. In unserem Fall
        /// rufen wir Ping auf. In der Windows Konsole kann mit
        ///     tasklist | find /I "PING.EXE"
        /// geprüft werden, wie viele Ping Prozesse laufen.
        /// </summary>
        private async Task StartJob(Jobinfo jobinfo)
        {
            using (var process = new Process())
            {
                var startTime = DateTime.UtcNow;
                process.StartInfo.FileName = "ping.exe";
                process.StartInfo.Arguments = $"-n 10 {jobinfo.Server}";
                //process.StartInfo.WorkingDirectory = "/a/directory";  // Wenn es benötigt wird, kann das Working Directory gesetzt werden.
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();
                using var stdoutReader = process.StandardOutput;
                using var stderrReader = process.StandardError;

                // Das Timeput ist ein Delay, welches wir - wenn der Prozess zuerst fertig wird - abbrechen.
                var timeoutToken = new CancellationTokenSource();
                // Damit keine TaskCancelException geworfen wird, setzen wir mit einem leeren Task fort.
                var timeoutTask = Task.Delay(_timeout, timeoutToken.Token).ContinueWith(task => { });
                // Jetzt wird der externe Prozess gestartet.
                var processTask = process.WaitForExitAsync();
                // Welcher Task ist zu erst fertig?
                var finishedTask = await Task.WhenAny(timeoutTask, processTask);
                if (finishedTask == timeoutTask)
                {
                    // Im Falle eines Timeout senden wir ein Kill. Prüfe in der gestarteten Applikation,
                    // ob in diesem Fall auch alle Ressourcen geschlossen werden.
                    process.Kill();
                    WriteJobFailedInfo(jobinfo, startTime, null, "Timeout. Process killed.");
                    return;
                }
                // Prozess war schneller als das Timeout? Den Timeout Task abbrechen (muss auch gemacht werden,
                // sonst würde dieser noch weiterlaufen und den Taskpool blockieren.
                timeoutToken.Cancel();
                await timeoutTask;
                // Hat der Prozess einen Exit Code != 0 geliefert, schreiben wir die Ausgabe und die
                // STDERR Ausgabe in die Datenbank. Prüfe im aufgerufenen Prozess, ob dieser auch saubere
                // Exit Codes liefert (0 = Success, ungleich 0 im Fehlerfall)
                if (process.ExitCode != 0)
                {
                    var message = $"{stdoutReader.ReadToEnd()}{Environment.NewLine}{stderrReader.ReadToEnd()}";
                    WriteJobFailedInfo(jobinfo, startTime, process.ExitCode, message);
                    return;
                }
                ProcessJobResult(jobinfo, startTime, stdoutReader.ReadToEnd());
            }
        }

        /// <summary>
        /// Parst die Ausgabe des Prozesses und führt weitere Verarbeitungsschritte
        /// wie das Schreiben in die Datenbank durch.
        /// </summary>
        private void ProcessJobResult(Jobinfo jobinfo, DateTime startTime, string output)
        {
            // Parse output
            var job = new Job(username:
                jobinfo.Username,
                server: jobinfo.Server,
                startTime: startTime,
                exitCode: 0);

            // Ping liefert Ausgaben wie
            // Reply from 194.232.104.140: bytes=32 time=21ms TTL=55
            // in der englischen Version. Wir suchen nach Zeilen mit time oder Zeit und lesen den Wert.
            var timeExp = new Regex(@"(time|Zeit)=(?<ms>[0-9]+)ms");
            var pingResults = timeExp.Matches(output).Select(m => new PingResult(
                time: int.TryParse(m.Groups["ms"].Value, out var ms) ? ms : 0,
                job: job));

            // In einem Singleton Service müssen wir einen eigenen Scope für den DB Context
            // erstellen, da der Context kein Singleton Service ist.
            try
            {
                using var serviceScope = _serviceScopeFactory.CreateScope();
                using var db = serviceScope.ServiceProvider.GetRequiredService<PingContext>();
                db.Jobs.Add(job);
                db.PingResults.AddRange(pingResults);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Database error in ProcessJobResult.");
            }
        }

        /// <summary>
        /// Schreibt Infos in die Datenbank, wenn etwas schiefgegangen ist. Das ist 
        /// sehr wichtig, da wir asynchron arbeiten und dem User nicht direkt einen Fehler
        /// rückmelden können.
        /// </summary>
        private void WriteJobFailedInfo(Jobinfo jobinfo, DateTime startTime, int? exitCode, string errorMessage)
        {
            try
            {
                using var serviceScope = _serviceScopeFactory.CreateScope();
                using var db = serviceScope.ServiceProvider.GetRequiredService<PingContext>();
                var job = new Job(username:
                    jobinfo.Username,
                    server: jobinfo.Server,
                    startTime: startTime,
                    exitCode: exitCode,
                    errorMessage: errorMessage);
                db.Jobs.Add(job);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Database error in WriteJobFailedInfo.");
            }
        }
    }
}
