namespace ExamManager.App.Entities
{
    public class CommitedExam : Exam
    {
        private CommitedExam()
        {

        }
        public CommitedExam(Exam e, string room) : base(
            teacherId: e.TeacherId,
            subjectId: e.SubjectId,
            date: e.Date,
            schoolClassName: e.SchoolClassName)
        {
            TeacherId = e.TeacherId;
            SubjectId = e.SubjectId;
            Date = e.Date;
            Room = room;
        }
        public string Room { get; set; } = default!;
    }

}
