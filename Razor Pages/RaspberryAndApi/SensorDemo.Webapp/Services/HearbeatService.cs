using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SensorDemo.Webapp.Services
{
    /// <summary>
    /// Service zum Überprüfen des letzten angekommenen Heartbeats. Damit es über DI genutzt werden
    /// kann, muss es mit
    ///     services.AddSingleton<HearbeatService>();
    /// als Singleton registriert werden. Danach muss in der Main Methode mit 
    ///     var host = CreateHostBuilder(args).Build();
    ///     var service = host.Services.GetRequiredService<HearbeatService>();
    ///     service.StartHearbeatService();
    ///     host.Run();
    /// vor dem Server gestartet werden. Sonst startet das Singleton Service erst bei der ersten
    /// Anforderung durch das DI System.
    /// </summary>
    public class HearbeatService
    {
        private readonly CancellationToken _cancellationToken;
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public HearbeatService(IHostApplicationLifetime applicationLifetime)
        {
            _cancellationToken = applicationLifetime.ApplicationStopping;
        }

        private DateTime _lastHeartbeat = DateTime.UtcNow;
        private static readonly TimeSpan _maxMissing = TimeSpan.FromSeconds(10);
        /// <summary>
        /// Startpunkt für das Service. Wird in Main() aufgerufen und erstellt den Thread
        /// für die Execute Methode.
        /// </summary>
        public void StartHearbeatService()
        {
            Task.Run(ExecuteAsync);
        }
        /// <summary>
        /// Setzt den letzten Heartbeat auf die aktuelle Zeit.
        /// </summary>
        public void SetHeartbeat(double timestamp)
        {
            DateTime raspTime = _epoch.AddSeconds(timestamp);
            Console.WriteLine($"Offset: {(DateTime.UtcNow - raspTime).TotalSeconds} sec.");
            _lastHeartbeat = DateTime.UtcNow;
        }

        /// <summary>
        /// Endlosschleife, die die Aufgaben des Background Services beinhaltet.
        /// </summary>
        private async Task ExecuteAsync()
        {
            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    Console.Write("Is raspberry alive? ");
                    if (DateTime.UtcNow - _lastHeartbeat > _maxMissing)
                    {
                        Console.WriteLine("He's dead :´(");
                    }
                    else
                    {
                        Console.WriteLine("He's alive :)");
                    }
                    await Task.Delay(1000, _cancellationToken);
                }
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine("Shut down. Clean up...");
            }
        }
    }
}
