using System;
using System.Collections.Generic;

namespace Plf4chif.Test.Model
{
    public partial class Pruefung
    {
        public long P_ID { get; set; }
        public DateTime P_Datum { get; set; }
        public long P_Schueler { get; set; }
        public string P_Fach { get; set; }
        public long? P_Note { get; set; }

        public virtual Fach P_FachNavigation { get; set; }
        public virtual Schueler P_SchuelerNavigation { get; set; }
    }
}
