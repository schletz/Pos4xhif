using ExamManager.App.Entities;
using System.Linq;

namespace ExamManager.App.Repositories
{
    public class ExamRepository : Repository<Exam, int>
    {
        public ExamRepository(ExamContext db) : base(db) { }
        public override (bool success, string message) Insert(Exam exam)
        {
            if (_db.Exam.Count(e => e.SchoolClassName == exam.SchoolClassName &&
            e.Date == exam.Date) > 2)
            {
                return (false, "To many exams in this class on that day");
            }
            return base.Insert(exam);

        }
    }
}
