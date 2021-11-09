namespace ExamManager.App.Entities
{
    /// <summary>
    /// An exam committed by the teacher. Adds a room to
    /// the planned exam.
    /// </summary>
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
