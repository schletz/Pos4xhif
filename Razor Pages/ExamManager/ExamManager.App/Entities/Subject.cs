using System;
using System.ComponentModel.DataAnnotations;
namespace ExamManager.App.Entities
{
    public class Subject : IEntity<int>
    {
        public Subject(string shortname, string name)
        {
            Shortname = shortname;
            Name = name;
        }

        protected Subject() { }  // For EF Core proxies
        public int Id { get; private set; }
        public Guid Guid { get; private set; }
        [Key]
        public string Shortname { get; set; } = default!;
        public string Name { get; set; } = default!;
    }

}
