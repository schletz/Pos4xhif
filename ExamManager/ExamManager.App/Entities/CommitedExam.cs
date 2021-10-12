namespace ExamManager.App.Entities
{
    public class CommitedExam : Exam
    {
        private CommitedExam()
        {

        }
        public CommitedExam(Exam e, string room) : base(
            teacherShortname: e.TeacherShortname,
            subjectShortname: e.SubjectShortname,
            date: e.Date,
            schoolClassName: e.SchoolClassName)
        {
            Room = room;
        }
        public string Room { get; set; } = default!;
    }

}
