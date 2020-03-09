using System;
using System.Collections.Generic;
using System.Text;

namespace TestAdministrator.Dto
{
    public class UserDto
    {
        public enum Userrole { Pupil = 1, Teacher}
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Token { get; set; } = "";
        public Userrole Role { get; set; } = 0;
        public string TeacherId { get; set; }
        public long? PupilId { get; set; }
    }
}
