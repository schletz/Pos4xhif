namespace ExamManager.App.Entities
{
    public class CommitedExam : Exam
    {
        private CommitedExam()
        {

        }
        public CommitedExam(Exam e, string room)
        {
            Id = e.Id;
            TeacherId = e.TeacherId;
            SubjectId = e.SubjectId;
            Date = e.Date;
            Room = room;
        }
        public string Room { get; set; } = default!;
    }

}
