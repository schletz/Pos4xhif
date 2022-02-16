using ExamManager.App.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamManager.Webapp.Pages.Exams
{
    public class StudentExamsModel : PageModel
    {
        private readonly ExamContext _db;

        public IReadOnlyList<Exam> Exams { get; private set; }
            = new List<Exam>();

        public string? Message { get; private set; }

        public StudentExamsModel(ExamContext db)
        {
            _db = db;
        }

        public IActionResult OnGet(Guid studentGuid)
        {
            var student = _db.Students.FirstOrDefault(s => s.Guid == studentGuid);
            if (student is null)
            {
                Message = "Student not found :(";
                return Page();
            }
            Exams = _db
                .Exam
                .Include(e => e.Teacher)
                .Include(e => e.Subject)
                .Where(e => e.SchoolClassId == student.SchoolClassId)
                .ToList();
            return Page();
        }
    }
}
