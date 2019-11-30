using System;
using System.Collections.Generic;
using System.Text;

namespace Plf4bhif20191125.Dto
{
    public class RegistrationDto
    {
        public long ID { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public DateTime Date_of_Birth { get; set; }
        public string Department { get; set; }
        public DateTime Registration_Date { get; set; }
        public DateTime? Admitted_Date { get; set; }
    }
}
