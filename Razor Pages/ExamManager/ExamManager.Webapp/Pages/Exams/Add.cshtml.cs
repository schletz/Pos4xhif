using AutoMapper;
using ExamManager.App.Dtos;
using ExamManager.App.Entities;
using ExamManager.App.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace ExamManager.Webapp.Pages.Exams
{
    public class AddModel : PageModel
    {
        public ExamRepository _exams;
        public IMapper _mapper;

        public AddModel(ExamRepository exams, IMapper mapper)
        {
            _exams = exams;
            _mapper = mapper;
        }

        public List<Exam> Exams { get; private set; } = new();
        [BindProperty]
        public ExamDto ExamDto { get; set; } = default!;
        public string? Message { get; private set; }
        public void OnGet()
        {
            Exams = _exams.Set
                .OrderBy(e => e.SchoolClassName)
                .ToList();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var exam = _mapper.Map<ExamDto, Exam>(ExamDto, opt =>
            {
                opt.AfterMap((dto, exam) =>
                {
                    exam.Teacher = _teachers.Set.FirstOrDefault(t => t.Guid == dto.TeacherGuid);
                    exam.SchoolClass = _classes.Set.FirstOrDefault(c => c.Guid == dto.SchoolClassGuid);
                    exam.SubjectGuid = _subjects.Set.FirstOrDefault(t => t.Guid == dto.SubjectGuid);
                });
            });
            var (success, message) = _exams.Insert(exam);
            if (!success)
            {
                Message = message;
            }
        }
    }
}
