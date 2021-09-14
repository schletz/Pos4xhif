using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Entities
{
    // POCO class (= plain old clr class)
    // POJO class (= plain old java class)
    public class Student
    {
        public Student(string account, string lastname, string firstname, Address home)
        {
            Account = account;
            Lastname = lastname;
            Firstname = firstname;
            Home = home;
        }
        private Student() { }
        // Id -> primary key, int Id -> autoincrement
        public int Id { get; private set; }
        public string Account { get; set; } = default!;
        public string Lastname { get; set; } = default!;
        public string Firstname { get; set; } = default!;
        public string? Email { get; set; }

        // Home address
        public Address Home { get; set; } = default!;
        public Address? Parents { get; set; }

        private int MailLength => string.IsNullOrEmpty(Email) ? 0 : Email.Length;
    }

}
