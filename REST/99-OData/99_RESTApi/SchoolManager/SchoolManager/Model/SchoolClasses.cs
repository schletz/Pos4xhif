using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SchoolManager.Model
{
    public partial class SchoolClasses
    {
        public SchoolClasses()
        {
            Pupils = new HashSet<Pupils>();
        }

        [Key]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Department { get; set; }

        [InverseProperty("SchoolClass")]
        public virtual ICollection<Pupils> Pupils { get; set; }
    }
}
