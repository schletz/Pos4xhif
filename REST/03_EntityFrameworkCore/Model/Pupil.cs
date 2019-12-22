using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFrameworkCore.Model
{
    public partial class Pupil
    {
        [Key]
        [System.Text.Json.Serialization.JsonPropertyName("Id")]
        public long P_ID { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(16)")]
        [System.Text.Json.Serialization.JsonPropertyName("Account")]
        public string P_Account { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        [System.Text.Json.Serialization.JsonPropertyName("Lastname")]
        public string P_Lastname { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(100)")]
        [System.Text.Json.Serialization.JsonPropertyName("Firstname")]
        public string P_Firstname { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(8)")]
        [System.Text.Json.Serialization.JsonPropertyName("Class")]
        public string P_Class { get; set; }

        [ForeignKey(nameof(P_Class))]
        [InverseProperty(nameof(Schoolclass.Pupil))]
        [System.Text.Json.Serialization.JsonIgnore]
        public virtual Schoolclass P_ClassNavigation { get; set; }
    }
}
