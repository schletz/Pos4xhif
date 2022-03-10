using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessDemo.Model
{
    public class Job
    {
        public Job(string username, string server, DateTime startTime, int? exitCode = null, string? errorMessage = null)
        {
            Username = username;
            Server = server;
            StartTime = startTime;
            ExitCode = exitCode;
            ErrorMessage = errorMessage;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Job() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public string Username { get; set; }
        public string Server { get; set; }
        public DateTime StartTime { get; set; }
        public int? ExitCode { get; set; }
        public string? ErrorMessage { get; set; }
        public ICollection<PingResult> PingResults { get; } = new List<PingResult>();
    }
}
