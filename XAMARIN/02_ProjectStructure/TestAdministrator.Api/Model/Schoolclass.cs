using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestAdministrator.Api.Model
{
    public partial class Schoolclass
    {
        public Schoolclass()
        {
            Lesson = new HashSet<Lesson>();
            Pupil = new HashSet<Pupil>();
            Test = new HashSet<Test>();
        }

        [Key]
        [Column(TypeName = "VARCHAR(8)")]
        public string C_ID { get; set; }
        [Required]
        [Column(TypeName = "VARCHAR(8)")]
        public string C_Department { get; set; }
        [Column(TypeName = "VARCHAR(8)")]
        public string C_ClassTeacher { get; set; }

        [ForeignKey(nameof(C_ClassTeacher))]
        [InverseProperty(nameof(Teacher.Schoolclass))]
        public virtual Teacher C_ClassTeacherNavigation { get; set; }
        [InverseProperty("L_ClassNavigation")]
        public virtual ICollection<Lesson> Lesson { get; set; }
        [InverseProperty("P_ClassNavigation")]
        public virtual ICollection<Pupil> Pupil { get; set; }
        [InverseProperty("TE_ClassNavigation")]
        public virtual ICollection<Test> Test { get; set; }
    }
}
