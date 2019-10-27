using System;
using System.Collections.Generic;

namespace MasterDetailDemo.Dto
{
    public partial class TestDto
    {
        public long ID { get; set; }
        public string Class { get; set; }
        public string Teacher { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public long Lesson { get; set; }
    }
}
