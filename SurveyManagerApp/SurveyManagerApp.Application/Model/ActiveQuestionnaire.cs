using System;

namespace SurveyManagerApp.Application.Model
{
    public class ActiveQuestionnaire : QuestionnaireState
    {
        public ActiveQuestionnaire(DateTime activeTo)
        {
            ActiveTo = activeTo;
        }

#pragma warning disable CS8618
        protected ActiveQuestionnaire() { }
#pragma warning restore CS8618 
        public DateTime ActiveTo { get; set; }
    }
}
