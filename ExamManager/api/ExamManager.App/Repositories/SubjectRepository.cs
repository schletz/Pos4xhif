using ExamManager.App.Entities;

namespace ExamManager.App.Repositories
{
    public class SubjectRepository : Repository<Subject, int>
    {
        public SubjectRepository(ExamContext db) : base(db)
        {
        }
    }
}
