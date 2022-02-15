using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Dtos
{
    public record ExamDto(
        Guid Guid,
        Guid TeacherGuid,  // GUID for Foreign keys!!
        Guid SubjectGuid,
        Guid SchoolClassGuid,
        [Range(typeof(DateTime), "2000-01-01", "2100-01-01", ErrorMessage = "Invalid date"]
        DateTime Date
        )
    {

    }
}
