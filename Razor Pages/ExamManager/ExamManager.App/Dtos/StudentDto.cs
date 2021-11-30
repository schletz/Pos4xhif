using ExamManager.App.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Dtos
{
    public record StudentDto(
        Guid Guid,
        [RegularExpression(@"^[A-Z]{3}[0-9]+", ErrorMessage = "Invalid Accountname")]
        string Account,
        [StringLength (255, MinimumLength = 2, ErrorMessage ="Invalid Lastname")]
        string Lastname,
        string Firstname,

        DateTime DateOfBirth,
        string? Email,
        Address Home,
        Address? Parents,
        string SchoolClassName);

}
