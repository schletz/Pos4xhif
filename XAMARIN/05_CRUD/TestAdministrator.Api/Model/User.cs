using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TestAdministrator.Api.Model
{
    public class User
    {
        [Key]
        [Column("U_Name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Column("U_Salt")]
        [StringLength(24)]
        [Required]
        public string Salt { get; set; }
        [Column("U_Hash")]
        [StringLength(44)]
        public string Hash { get; set; }
        [Column("U_LastLogin", TypeName="DATETIME")]
        public DateTime? LastLogin { get; set; }
    }
}
