using ExamManager.App.Entities;

namespace ExamManager.App.Repositories
{
    public class SchoolClassRepository : Repository<SchoolClass, int>
    {
        public SchoolClassRepository(ExamContext db) : base(db)
        {
        }
    }
}
