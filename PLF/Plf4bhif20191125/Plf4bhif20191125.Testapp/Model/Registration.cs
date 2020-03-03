using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plf4bhif20191125.Testapp.Model
{
    public partial class Registration
    {
        [Key]
        public long R_ID { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string R_Firstname { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string R_Lastname { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string R_Email { get; set; }
        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime R_Date_of_Birth { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(10)")]
        public string R_Department { get; set; }
        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime R_Registration_Date { get; set; }
        [Column(TypeName = "TIMESTAMP")]
        public DateTime? R_Admitted_Date { get; set; }

        [ForeignKey(nameof(R_Department))]
        [InverseProperty(nameof(Department.Registration))]
        public virtual Department R_DepartmentNavigation { get; set; }
    }
}
