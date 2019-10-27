using System;
using System.Collections.Generic;

namespace MasterDetailDemo.Api.Model
{
    public partial class Pupil
    {
        public long P_ID { get; set; }
        public string P_Account { get; set; }
        public string P_Lastname { get; set; }
        public string P_Firstname { get; set; }
        public string P_Class { get; set; }

        public virtual Schoolclass P_ClassNavigation { get; set; }
    }
}
