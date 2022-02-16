using ExamManager.App.Entities;

namespace ExamManager.App.Repositories
{
    public class TeacherRepository : Repository<Teacher, int>
    {
        public TeacherRepository(ExamContext db) : base(db)
        {
        }
    }
}
