using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
namespace ExamManager.App.Entities
{
    public class Exam : IEntity<int>
    {
        public Exam(
            Teacher teacher,
            Subject subject,
            DateTime date,
            SchoolClass schoolClass)
        {
            Teacher = teacher;
            TeacherId = teacher.Id;
            Subject = subject;
            SubjectId = subject.Id;
            SchoolClass = schoolClass;
            SchoolClassId = SchoolClass.Id;
            Date = date;
            Guid = Guid.NewGuid();
        }
        protected Exam() { }
        public int Id { get; protected set; }
        public Guid Guid { get; protected set; }
        // Name of the navigation property + name of the PK
        public int TeacherId { get; set; }
        public virtual Teacher Teacher { get; set; } = default!; 
        public int SubjectId { get; set; } = default!;
        // Virtual for EF Core Proxies (requires NuGet Microsoft.EntityFrameworkCore.Proxies)
        public virtual Subject Subject { get; set; } = default!;

        public int SchoolClassId { get; set; } = default!;
        // Virtual for EF Core Proxies (requires NuGet Microsoft.EntityFrameworkCore.Proxies)
        public virtual SchoolClass SchoolClass { get; set; } = default!;
        public DateTime Date { get; set; }
    }

}
