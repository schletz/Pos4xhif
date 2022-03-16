using AutoMapper;
using ExamManager.App.Dtos;
using ExamManager.App.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamManager.App.Mappings
{
    public class DtoMappings : Profile
    {
        public DtoMappings()
        {
            CreateMap<StudentDto, Student>();  // StudentDto --> Student
            CreateMap<Student, StudentDto>();  // Student --> StudentDto
            // !! NOTICE: Use AfterMap to assign Teacher, Subject, Class
            CreateMap<ExamDto, Exam>();
            // TecherGuid will be initialized (flattening), because AutoMapper
            // reads TeacherGuid as Teacher + Guid.
            CreateMap<Exam, ExamDto>();
        }

    }
}
