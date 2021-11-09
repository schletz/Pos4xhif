using System.ComponentModel.DataAnnotations;
namespace ExamManager.App.Entities
{
    public class Subject
    {
        public Subject(string shortname, string name)
        {
            Shortname = shortname;
            Name = name;
        }

        protected Subject() { }  // For EF Core proxies
        [Key]
        public string Shortname { get; set; } = default!;
        public string Name { get; set; } = default!;
    }

}
