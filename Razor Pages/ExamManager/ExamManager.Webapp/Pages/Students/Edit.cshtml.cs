using AutoMapper;
using ExamManager.App.Dtos;
using ExamManager.App.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamManager.Webapp.Pages.Students
{
    public class EditModel : PageModel
    {
        private readonly ExamContext _db;
        private readonly IMapper _mapper;

        public string? Message { get; set; }
        [BindProperty]
        public StudentDto Student { get; set; } = default!;
        public IEnumerable<SelectListItem> Classes { get; private set; } = Enumerable.Empty<SelectListItem>();

        public EditModel(ExamContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public IActionResult OnGet(Guid guid)
        {
            var student = _db.Students.FirstOrDefault(s => s.Guid == guid);
            if (student is null)
            {
                Message = "Student not found.";
                return Page();
            }
            Student = _mapper.Map<StudentDto>(student);
            return Page();
        }

        public IActionResult OnPost(Guid guid)
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
            var studentDb = _db.Students.FirstOrDefault(s => s.Guid == guid);
            if (studentDb is null)
            {
                Message = "Student not found";
                return Page();
            }
            studentDb.Account = Student.Account;
            studentDb.DateOfBirth = Student.DateOfBirth;
            studentDb.Email = Student.Email;
            studentDb.Lastname = studentDb.Lastname;
            studentDb.Firstname = studentDb.Firstname;
            studentDb.Home = studentDb.Home;
            studentDb.Parents = studentDb.Parents;
            studentDb.SchoolClass = schoolClass;
            _db.SaveChanges();

            // REDIRECT AFTER POST
            //return Page();
            return RedirectToPage("/Students/Index");
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            Classes = _db.SchoolClasses
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                })
                .OrderBy(o => o.Text);
        }

    }
}
