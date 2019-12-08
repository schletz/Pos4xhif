using System;
using System.Collections.Generic;

namespace SPG.CodeFirstApplication.Entities
{
    public class SchoolClass
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Department { get; set; }

        public virtual List<Pupil> Pupils { get; set; } = new List<Pupil>();
    }
}
