using System;
using System.Collections.Generic;

namespace TestAdministrator.Api.Model
{
    public partial class Teacher
    {
        public Teacher()
        {
            Lesson = new HashSet<Lesson>();
            Schoolclass = new HashSet<Schoolclass>();
            Test = new HashSet<Test>();
        }

        public string T_ID { get; set; }
        public string T_Lastname { get; set; }
        public string T_Firstname { get; set; }
        public string T_Email { get; set; }
        public string T_Account { get; set; }

        public virtual ICollection<Lesson> Lesson { get; set; }
        public virtual ICollection<Schoolclass> Schoolclass { get; set; }
        public virtual ICollection<Test> Test { get; set; }
    }
}
