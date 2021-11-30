using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Entities
{
    // POCO class (= plain old clr class)
    // POJO class (= plain old java class)
    public class Student
    {
        public Student(
            string account, string lastname,
            string firstname, Address home, DateTime dateOfBirth, SchoolClass schoolClass)
        {
            Account = account;
            Lastname = lastname;
            Firstname = firstname;
            Home = home;
            DateOfBirth = dateOfBirth;
            Guid = Guid.NewGuid();
            SchoolClass = schoolClass;
        }
        protected Student() { }
        // Id -> primary key, int Id -> autoincrement
        public int Id { get; private set; }
        public Guid Guid { get; private set; }
        [MaxLength(255)]
        public string Account { get; set; } = default!;
        [MaxLength(255)]
        public string Lastname { get; set; } = default!;
        [MaxLength(255)]
        public string Firstname { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        [MaxLength(255)]
        public string? Email { get; set; }
        // Home address
        public Address Home { get; set; } = default!;
        public Address? Parents { get; set; }

        public string SchoolClassName { get; set; } = default!;  // FK Value (4EHIF, ...)
        public virtual SchoolClass SchoolClass { get; set; } = default!; // Navigation to class

        private int MailLength => string.IsNullOrEmpty(Email) ? 0 : Email.Length;
    }

}
