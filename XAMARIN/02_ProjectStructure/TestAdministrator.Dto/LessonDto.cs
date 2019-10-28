using System;
using System.Collections.Generic;

namespace TestAdministrator.Dto
{
    public partial class LessonDto
    {
        public long Id { get; set; }
        public long? Untis_ID { get; set; }
        public string Class { get; set; }
        public string Teacher { get; set; }
        public string Subject { get; set; }
        public string Room { get; set; }
        public long? Day { get; set; }
        public long? Hour { get; set; }
    }
}
