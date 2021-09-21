using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Entities
{
    public class Exam
    {
        public Exam(int id, string teacherId, string subjectId, DateTime date)
        {
            Id = id;
            TeacherId = teacherId;
            SubjectId = subjectId;
            Date = date;
        }
        private Exam() { }
        public int Id { get; private set; }
        public string TeacherId { get; set; } = default!;  // TODO: Define a teacher class
        public string SubjectId { get; set; } = default!;  // TODO: Define a subject class
        public DateTime Date { get; set; }
    }


}
