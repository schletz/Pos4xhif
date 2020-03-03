using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Plf4ahif20191118.Model
{
    public partial class School_Rating
    {
        [Key]
        public long SR_ID { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(255)")]
        public string SR_User_Phonenr { get; set; }
        public long SR_User_School { get; set; }
        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime SR_Date { get; set; }
        public long SR_Value { get; set; }

        [ForeignKey("SR_User_Phonenr,SR_User_School")]
        [InverseProperty(nameof(User.School_Rating))]
        public virtual User SR_User_Navigation { get; set; }
    }
}
