using System;
using System.Collections.Generic;

namespace MasterDetailDemo.Api.Model
{
    public partial class Period
    {
        public Period()
        {
            Lesson = new HashSet<Lesson>();
            Test = new HashSet<Test>();
        }

        public long P_Nr { get; set; }
        public DateTime P_From { get; set; }
        public DateTime P_To { get; set; }

        public virtual ICollection<Lesson> Lesson { get; set; }
        public virtual ICollection<Test> Test { get; set; }
    }
}
