using System;
using System.Collections.Generic;

namespace Plf4chif.Test.Model
{
    public partial class Klasse
    {
        public Klasse()
        {
            Schueler = new HashSet<Schueler>();
        }

        public string K_Name { get; set; }
        public string K_Abteilung { get; set; }

        public virtual ICollection<Schueler> Schueler { get; set; }
    }
}
