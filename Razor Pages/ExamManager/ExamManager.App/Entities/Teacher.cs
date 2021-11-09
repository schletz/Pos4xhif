using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExamManager.App.Entities
{
    public class Teacher
    {
        public Teacher(string shortname, string firstname, string lastname, string email)
        {
            Shortname = shortname;
            Firstname = firstname;
            Lastname = lastname;
            Email = email;
        }

        protected Teacher() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]         // by convention (string datatype)
        public string Shortname { get; private set; } = default!; // Shortname (SZ)
        public string Firstname { get; set; } = default!;
        public string Lastname { get; set; } = default!;
        public string Email { get; set; } = default!;
    }

}
