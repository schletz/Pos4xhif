using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SurveyManagerApp.Application.Model
{
    public class Student : IEntity<int>
    {
        public Student(int number, Name name, string email, Role role, Schoolclass schoolclass)
        {
            Number = number;
            Name = name;
            Email = email;
            Role = role;
            Schoolclass = schoolclass;
        }
#pragma warning disable CS8618 
        protected Student() { }
#pragma warning restore CS8618
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Number { get; private set; }  // From school mgmt software
        public Name Name { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
        public Role Role { get; set; }
        // Convention: PropertyName of navigation + PK name of Schoolclass
        public int SchoolclassId { get; set; }        // FK VALUE of Schoolclass
        public Schoolclass Schoolclass { get; set; }  // Navigation to Schoolclass
        public int Id => Number;                 // Because we have to implement IEntity
    }
}
