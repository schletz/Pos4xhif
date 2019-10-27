using System;
using System.Collections.Generic;

namespace MasterDetailDemo.Api.Model
{
    public partial class Schoolclass
    {
        public Schoolclass()
        {
            Lesson = new HashSet<Lesson>();
            Pupil = new HashSet<Pupil>();
            Test = new HashSet<Test>();
        }

        public string C_ID { get; set; }
        public string C_Department { get; set; }
        public string C_ClassTeacher { get; set; }

        public virtual Teacher C_ClassTeacherNavigation { get; set; }
        public virtual ICollection<Lesson> Lesson { get; set; }
        public virtual ICollection<Pupil> Pupil { get; set; }
        public virtual ICollection<Test> Test { get; set; }
    }
}
