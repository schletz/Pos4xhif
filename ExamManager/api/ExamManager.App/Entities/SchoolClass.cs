using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Entities
{
    public class SchoolClass : IEntity<int>
    {
        public SchoolClass(string name, Teacher classRepresentative, string? room = null)
        {
            Name = name;
            ClassRepresentative = classRepresentative;
            Room = room;
        }

        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected SchoolClass() { }
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


        public int Id { get; private set; }
        public Guid Guid { get; private set; }
        public string Name { get; set; } = default!;  // 4EHIF, ..
        public string? Room { get; set; }
        public int ClassRepresentativeId { get; set; }
        public virtual Teacher ClassRepresentative { get; set; }
        public virtual ICollection<Student> Students { get; } = new List<Student>(0);
        public virtual ICollection<Exam> Exams { get; } = new List<Exam>(0);
        // Test for EF Core LazyLoading: Will produce a SQL Statement because
        // our students are in a seperate table.
        public int StudentsCount => Students.Count();
    }
}
