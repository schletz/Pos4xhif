namespace ExamManager.App.Entities
{
    /// <summary>
    /// An exam committed by the teacher. Adds a room to
    /// the planned exam.
    /// </summary>
    public class CommitedExam : Exam
    {
        protected CommitedExam() { }
        public CommitedExam(Exam e, string room) : base(
            teacher: e.Teacher,
            subject: e.Subject,
            date: e.Date,
            schoolClass: e.SchoolClass)
        {
            Room = room;
        }
        public string Room { get; set; } = default!;
    }

}
