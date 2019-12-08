using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plf4ahif20191118.Testapp.Model
{
    public partial class School
    {
        public School()
        {
            User = new HashSet<User>();
        }

        [Key]
        public long S_Nr { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string S_Name { get; set; }

        [InverseProperty("U_SchoolNavigation")]
        public virtual ICollection<User> User { get; set; }
    }
}
