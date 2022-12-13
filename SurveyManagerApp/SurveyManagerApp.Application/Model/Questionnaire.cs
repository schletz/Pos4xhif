namespace SurveyManagerApp.Application.Model
{
    public class Questionnaire
    {
        public Questionnaire(Schoolclass schoolclass, QuestionnaireState state)
        {
            Schoolclass = schoolclass;
            StateId = state.Id;
            State = state;
        }
        #pragma warning disable CS8618
        protected Questionnaire() { }
        #pragma warning restore CS8618
        public int Id { get; set; }
        public Schoolclass Schoolclass { get; set; }
        public int StateId { get; set; }
        public QuestionnaireState State { get; set; }

    }
}
