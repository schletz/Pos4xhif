using System;
using System.Collections.Generic;

namespace MasterDetailDemo.Api.Model
{
    public partial class Lesson
    {
        public long L_ID { get; set; }
        public long? L_Untis_ID { get; set; }
        public string L_Class { get; set; }
        public string L_Teacher { get; set; }
        public string L_Subject { get; set; }
        public string L_Room { get; set; }
        public long? L_Day { get; set; }
        public long? L_Hour { get; set; }

        public virtual Schoolclass L_ClassNavigation { get; set; }
        public virtual Period L_HourNavigation { get; set; }
        public virtual Teacher L_TeacherNavigation { get; set; }
    }
}
