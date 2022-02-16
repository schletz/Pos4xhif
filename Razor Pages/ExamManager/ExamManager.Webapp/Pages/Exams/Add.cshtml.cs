using AutoMapper;
using ExamManager.App.Dtos;
using ExamManager.App.Entities;
using ExamManager.App.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamManager.Webapp.Pages.Exams
{
    public class AddModel : PageModel
    {
        public ExamRepository _exams;
        public TeacherRepository _teachers;
        public SchoolClassRepository _schoolClasses;
        public SubjectRepository _subjects;
        public IMapper _mapper;

        public AddModel(
            ExamRepository exams,
            TeacherRepository teachers,
            SchoolClassRepository schoolClasses,
            SubjectRepository subjects,
            IMapper mapper)
        {
            _exams = exams;
            _teachers = teachers;
            _schoolClasses = schoolClasses;
            _subjects = subjects;
            _mapper = mapper;
        }

        public List<Exam> Exams { get; private set; } = new();
        [BindProperty]
        public ExamDto ExamDto { get; set; } = default!;
        public string? Message { get; private set; }
        public void OnGet()
        {
            Exams = _exams.Set
                .OrderBy(e => e.SchoolClassId)
                .ToList();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var teacher = _teachers.Set.FirstOrDefault(t => t.Guid == ExamDto.TeacherGuid) ?? throw new ApplicationException("Teacher does not exist.");
                var schoolClass = _schoolClasses.Set.FirstOrDefault(c => c.Guid == ExamDto.SchoolClassGuid) ?? throw new ApplicationException("Class does not exist.");
                var subject = _subjects.Set.FirstOrDefault(t => t.Guid == ExamDto.SubjectGuid) ?? throw new ApplicationException("Subject does not exist.");

                var exam = _mapper.Map<ExamDto, Exam>(ExamDto, opt =>
                {
                    opt.AfterMap((dto, exam) =>
                    {
                        exam.Teacher = teacher;
                        exam.SchoolClass = schoolClass;
                        exam.Subject = subject;
                    });
                });
                var (success, message) = _exams.Insert(exam);
                if (!success)
                {
                    throw new ApplicationException(message);
                }
            }
            catch (ApplicationException e)
            {
                Message = e.Message;
                return Page();
            }
            return RedirectToPage();

        }
    }
}
