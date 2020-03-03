using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plf4chif.Api.Model
{
    [Table("Pruefung")]

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
