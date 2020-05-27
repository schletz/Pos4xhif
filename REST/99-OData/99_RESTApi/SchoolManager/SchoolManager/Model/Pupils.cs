using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace SchoolManager.Model
{
    public partial class Pupils
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Gender { get; set; }
        [Required]
        public string SchoolClassId { get; set; }

        [ForeignKey(nameof(SchoolClassId))]
        [InverseProperty(nameof(SchoolClasses.Pupils))]
        [JsonIgnore]
        public virtual SchoolClasses SchoolClass { get; set; }
    }
}
