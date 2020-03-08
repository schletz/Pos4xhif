using System;

namespace TestAdministrator.Dto
{
    public class TestDto
    {
        public string Schoolclass { get; set; }
        public string Teacher { get; set; }
        public string Subject { get; set; }
        public DateTime DateFrom { get; set; }
        public long? Lesson { get; set; }
        public long? TestId { get; set; }
    }
}