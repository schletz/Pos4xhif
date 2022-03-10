using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessDemo.Model
{
    public class PingResult
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected PingResult() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public PingResult(int time, Job job)
        {
            Time = time;
            Job = job;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; private set; }
        public int Time { get; set; }
        public Guid JobGuid { get; set; }
        public Job Job { get; set; }
    }
}
