using System;

namespace SurveyManagerApp.Application.Model
{
    public class ClosedQuestionnaire : QuestionnaireState
    {
        public ClosedQuestionnaire(DateTime closingDate, Student closedFrom)
        {
            ClosingDate = closingDate;
            ClosedFrom = closedFrom;
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ClosedQuestionnaire() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public DateTime ClosingDate { get; set; }
        public Student ClosedFrom { get; set; }
    }
}
