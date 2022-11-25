using System.ComponentModel.DataAnnotations;

namespace SurveyManagerApp.Application.Model
{
    public class Schoolclass : IEntity<int>
    {
        public Schoolclass(string name)
        {
            Name = name;
        }
#pragma warning disable CS8618
        protected Schoolclass() { }         // Needed for ef core
#pragma warning restore CS8618
        public int Id { get; private set; } // Every key has private set
        [MaxLength(8)]
        public string Name { get; set; }
    }
}
