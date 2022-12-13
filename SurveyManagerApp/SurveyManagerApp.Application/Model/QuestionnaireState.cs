namespace SurveyManagerApp.Application.Model
{
    public class QuestionnaireState
    {
        public int Id { get; set; }
        public Questionnaire Questionnaire { get; set; } = default!;
    }
}
