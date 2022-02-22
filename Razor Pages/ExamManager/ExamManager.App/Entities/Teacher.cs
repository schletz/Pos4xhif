using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManager.App.Entities
{
    public class Teacher : IEntity<int>
    {
        public Teacher(string shortname, string firstname, string lastname, string email)
        {
            Shortname = shortname;
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
        }

        protected Teacher() { }
        public int Id { get; private set; }
        public Guid Guid { get; private set; }
        public string Shortname { get; private set; } = default!; // Shortname (SZ)
        public string Firstname { get; set; } = default!;
        public string Lastname { get; set; } = default!;
        public string Email { get; set; } = default!;
        public virtual ICollection<SchoolClass> SchoolClasses { get; } = new List<SchoolClass>();
    }

}
