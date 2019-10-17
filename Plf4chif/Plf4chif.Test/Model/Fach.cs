using System;
using System.Collections.Generic;

namespace Plf4chif.Test.Model
{
    public partial class Fach
    {
        public Fach()
        {
            Pruefung = new HashSet<Pruefung>();
        }

        public string F_ID { get; set; }
        public string F_Name { get; set; }

        public virtual ICollection<Pruefung> Pruefung { get; set; }
    }
}
