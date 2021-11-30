using ExamManager.App.Dtos;
using ExamManager.App.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace ExamManager.Webapp.Pages.Students
{
    public class AddModel : PageModel
    {
        private readonly ExamContext _db;

        public AddModel(ExamContext db)
        {
            _db = db;
        }

        [BindProperty]
        public StudentDto Student { get; set; } = new StudentDto(
            Guid: default,
            Account: string.Empty,
            Lastname: string.Empty,
            Firstname: string.Empty,
            DateOfBirth: default,
            Email: default,
            SchoolClassName: string.Empty,
            Home: new Address(string.Empty, string.Empty, string.Empty),
            Parents: default);
            

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var schoolClass = _db.SchoolClasses.FirstOrDefault(s => s.Name == Student.SchoolClassName);
            if (schoolClass is null)
            {
                ModelState.AddModelError(nameof(StudentDto.SchoolClassName), "Class not knwon.");
                return Page();

            }
            var student = new Student(
                account: Student.Account,
                lastname: Student.Lastname,
                firstname: Student.Firstname,
                home: Student.Home,
                dateOfBirth: Student.DateOfBirth,
                schoolClass: schoolClass);

            _db.Students.Add(student);
            _db.SaveChanges();
            return RedirectToPage("/Students/Index");
        }
    }
}
