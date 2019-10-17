using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plf4chif.Api.Model
{
    [Table("Fach")]
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
