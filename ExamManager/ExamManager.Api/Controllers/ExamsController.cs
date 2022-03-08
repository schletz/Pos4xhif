using AutoMapper;
using AutoMapper.QueryableExtensions;
using ExamManager.App.Dtos;
using ExamManager.App.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ExamManager.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly ExamContext _db;
        public readonly IMapper _mapper;

        public ExamsController(ExamContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        /// <summary>
        /// GET /api/exams
        /// </summary>
        [HttpGet]
        public IActionResult GetAllExams()
        {
            return Ok(_db.Exam.ProjectTo<ExamDto>(_mapper.ConfigurationProvider));
        }

        /// <summary>
        /// POST /api/exams
        /// </summary>
        /// <param name="examDto"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult InsertExam(ExamDto examDto)
        {
            // Look for foreign keys
            var teacher = _db.Teachers.FirstOrDefault(t => t.Guid == examDto.TeacherGuid);
            var subject = _db.Subjects.FirstOrDefault(s => s.Guid == examDto.SubjectGuid);
            var @class = _db.SchoolClasses.FirstOrDefault(s => s.Guid == examDto.SchoolClassGuid);
            if (teacher is null) { return BadRequest("Techer not found"); }
            if (subject is null) { return BadRequest("Subject not found"); }
            if (@class is null) { return BadRequest("´Class not found"); }

            // Assign navigation properties, because we are using auto increment
            // ids in your database and we receive only guid values.
            // Therefore automapper cannot map TeacherGuid to TeacherId automatically.
            var exam = _mapper.Map<Exam>(examDto, opts =>
            {
                opts.AfterMap((src, dest) =>
                {
                    dest.Teacher = teacher;
                    dest.Subject = subject;
                    dest.SchoolClass = @class;
                });
            });
            _db.Exam.Add(exam);
            _db.SaveChanges();
            // Return generated guid to the client to synchronize its state.
            return Ok(_mapper.Map<ExamDto>(exam));
        }


    }
}
