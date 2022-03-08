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
        public StudentDto StudentDto { get; set; } = default!;

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

            var schoolClass = _db.SchoolClasses.FirstOrDefault(s => s.Guid == StudentDto.SchoolClassGuid);
            if (schoolClass is null)
            {
                ModelState.AddModelError(nameof(App.Dtos.StudentDto.SchoolClassGuid), "Class not knwon.");
                return Page();
            }
            var student = _mapper.Map<StudentDto, Student>(StudentDto, opt => opt.AfterMap((src, dst) =>
             {
                 dst.SchoolClass = schoolClass;
             }));


            _db.Students.Add(student);  // INSERT INTO
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
                    Value = c.Guid.ToString(),
                    Text = c.Name
                })
                .OrderBy(o => o.Text);
        }
    }
}
