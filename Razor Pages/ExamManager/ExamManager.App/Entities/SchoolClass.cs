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

        [Key]      // ID -> PK by convention. Otherwise you have to declare your PK explicit.
        public string Name { get; private set; } = default!;  // 4EHIF, ..
        public string? Room { get; set; }
        public virtual ICollection<Student> Students { get; } = new List<Student>(0);
        public virtual ICollection<Exam> Exams { get; } = new List<Exam>(0);
        // Test for EF Core LazyLoading: Will produce a SQL Statement because
        // our students are in a seperate table.
        public int StudentsCount => Students.Count();
    }
}
