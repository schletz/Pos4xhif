using AutoMapper;
using ExamManager.App.Dtos;
using ExamManager.App.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;

namespace ExamManager.Webapp.Pages.Students
{
    public class AddModel : PageModel
    {
        private readonly ExamContext _db;
        private readonly IMapper _mapper;
        public AddModel(ExamContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [BindProperty]
        public StudentDto Student { get; set; } = default!;

        public IEnumerable<SelectListItem> Classes { get; private set; } = Enumerable.Empty<SelectListItem>();

        public void OnGet()
        {

        }
        // HANDLER for POST requests
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
            var student = _mapper.Map<Student>(Student);

            _db.Students.Add(student);
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
