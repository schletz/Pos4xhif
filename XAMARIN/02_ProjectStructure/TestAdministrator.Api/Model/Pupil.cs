using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestAdministrator.Api.Model
{
    public partial class Pupil
    {
        [Key]
        public long P_ID { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(16)")]
        public string P_Account { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string P_Lastname { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        public string P_Firstname { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(8)")]
        public string P_Class { get; set; }

        [ForeignKey(nameof(P_Class))]
        [InverseProperty(nameof(Schoolclass.Pupil))]
        public virtual Schoolclass P_ClassNavigation { get; set; }
    }
}
