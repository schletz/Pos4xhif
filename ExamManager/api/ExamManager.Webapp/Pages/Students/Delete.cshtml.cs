using ExamManager.App.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Linq;

namespace ExamManager.Webapp.Pages.Students
{
    public class DeleteModel : PageModel
    {
        private readonly ExamContext _db;

        public DeleteModel(ExamContext db)
        {
            _db = db;
        }

        public IActionResult OnGet(Guid guid)
        {
            var student = _db.Students.FirstOrDefault(s => s.Guid == guid);
            if (student is null)
            {
                return RedirectToPage("/Students/Index");
            }
            _db.Students.Remove(student);
            _db.SaveChanges();
            return RedirectToPage("/Students/Index");
        }
    }
}
