using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Entities
{
    public class SchoolClass
    {
        public SchoolClass(string name)
        {
            Name = name;
        }

        protected SchoolClass() { }

        [Key]
        public string Name { get; private set; } = default!;  // 4EHIF, ..
        public string? Room { get; set; }
        public virtual ICollection<Student> Students { get; } = new List<Student>(0);
        public virtual ICollection<Exam> Exams { get; } = new List<Exam>(0);
        public int StudentsCount => Students.Count();
    }
}
