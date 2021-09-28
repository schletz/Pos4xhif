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

        private SchoolClass() { }

        [Key]
        public string Name { get; private set; } = default!;  // 4EHIF, ..
        public string? Room { get; set; }
        public List<Student> Students { get; } = new(0);
        public List<Exam> Exams { get; } = new(0);

    }
}
