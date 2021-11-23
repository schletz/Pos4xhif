using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace ExamManager.App.Entities
{
    public class Exam
    {
        public Exam(
            string teacherShortname,
            string subjectShortname,
            DateTime date,
            string schoolClassName)
        {
            TeacherShortname = teacherShortname;
            SubjectShortname = subjectShortname;
            Date = date;
            SchoolClassName = schoolClassName;
            Guid = Guid.NewGuid();
        }
        protected Exam() { }
        public int Id { get; private set; }
        public Guid Guid { get; private set; }
        // Name of the navigation property + name of the PK
        public string TeacherShortname { get; set; } = default!;
        public virtual Teacher Teacher { get; set; } = default!; 
        public string SubjectShortname { get; set; } = default!;
        // Virtual for EF Core Proxies (requires NuGet Microsoft.EntityFrameworkCore.Proxies)
        public virtual Subject Subject { get; set; } = default!;

        public string SchoolClassName { get; set; } = default!;
        // Virtual for EF Core Proxies (requires NuGet Microsoft.EntityFrameworkCore.Proxies)
        public virtual SchoolClass SchoolClass { get; set; } = default!;
        public DateTime Date { get; set; }
    }

}
