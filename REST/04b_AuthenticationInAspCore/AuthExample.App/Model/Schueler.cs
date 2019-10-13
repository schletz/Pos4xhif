using System;
using System.Collections.Generic;

namespace AuthExample.App.Model
{
    public partial class Schueler
    {
        public Schueler()
        {
            User = new HashSet<User>();
        }

        public long S_Nr { get; set; }
        public string S_Klasse { get; set; }
        public string S_Zuname { get; set; }
        public string S_Vorname { get; set; }
        public string S_Geschl { get; set; }

        public virtual Klasse S_KlasseNavigation { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
