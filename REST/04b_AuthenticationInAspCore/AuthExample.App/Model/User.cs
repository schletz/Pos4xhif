using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthExample.App.Model
{
    public partial class User
    {
        public long U_ID { get; set; }
        public string U_Name { get; set; }
        public string U_Salt { get; set; }
        public string U_Hash { get; set; }
        public long? U_Schueler_Nr { get; set; }
        public string U_Role { get; set; }

        public virtual Schueler U_Schueler_NrNavigation { get; set; }
    }
}
