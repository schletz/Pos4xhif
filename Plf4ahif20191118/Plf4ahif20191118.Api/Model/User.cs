using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plf4ahif20191118.Model
{
    public partial class User
    {
        public User()
        {
            School_Rating = new HashSet<School_Rating>();
        }

        [Key]
        [Column(TypeName = "VARCHAR(255)")]
        public string U_Phonenr { get; set; }
        [Key]
        public long U_School { get; set; }

        [ForeignKey(nameof(U_School))]
        [InverseProperty(nameof(School.User))]
        public virtual School U_SchoolNavigation { get; set; }
        [InverseProperty("SR_User_Navigation")]
        public virtual ICollection<School_Rating> School_Rating { get; set; }
    }
}
