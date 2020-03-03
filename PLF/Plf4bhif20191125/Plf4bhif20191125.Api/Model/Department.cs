using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plf4bhif20191125.Model
{
    public partial class Department
    {
        public Department()
        {
            Registration = new HashSet<Registration>();
        }

        [Key]
        [Column(TypeName = "VARCHAR(10)")]
        public string D_Name { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string D_Longname { get; set; }

        [InverseProperty("R_DepartmentNavigation")]
        public virtual ICollection<Registration> Registration { get; set; }
    }
}
