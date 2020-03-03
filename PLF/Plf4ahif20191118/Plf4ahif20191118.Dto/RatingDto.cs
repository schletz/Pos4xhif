using System;
using System.Collections.Generic;
using System.Text;

namespace Plf4ahif20191118.Dto
{
    public class RatingDto
    {
        public long Id { get; set; }
        public string PhoneNr { get; set; }
        public long School { get; set; }
        public DateTime Date { get; set; }
        public long Value { get; set; }
    };
}
