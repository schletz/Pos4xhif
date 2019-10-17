﻿using System;
using System.Collections.Generic;

namespace Plf4chif.Test.Model
{
    public partial class Schueler
    {
        public Schueler()
        {
            Pruefung = new HashSet<Pruefung>();
        }

        public long S_Nr { get; set; }
        public string S_Klasse { get; set; }
        public string S_Zuname { get; set; }
        public string S_Vorname { get; set; }
        public string S_Geschl { get; set; }

        public virtual Klasse S_KlasseNavigation { get; set; }
        public virtual ICollection<Pruefung> Pruefung { get; set; }
    }
}
