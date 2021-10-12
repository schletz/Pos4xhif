using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Entities
{
    public class Exam
    {
        public Exam(string teacherId,
            string subjectId,
            DateTime date,
            string schoolClassName)
        {
            TeacherId = teacherId;
            SubjectId = subjectId;
            Date = date;
            SchoolClassName = schoolClassName;
        }
        protected Exam() { }
        public int Id { get; private set; }
        public string TeacherId { get; set; } = default!;  // TODO: Define a teacher class
        public string SubjectId { get; set; } = default!;  // TODO: Define a subject class
        public string SchoolClassName { get; set; } = default!;
        public virtual SchoolClass SchoolClass { get; set; } = default!;
        public DateTime Date { get; set; }
    }

}
