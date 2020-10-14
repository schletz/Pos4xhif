using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plf4bhif20191125.Dto;
using Plf4bhif20191125.Model;

namespace Plf4bhif20191125.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationContext _db;
        public RegistrationController(RegistrationContext db)
        {
            _db = db;
        }

        [HttpGet("test")]
        public ActionResult<RegistrationDto> GetTestdata()
        {
            return Ok(from r in _db.Registration
                      select new RegistrationDto
                      {
                          Firstname = r.R_Firstname,
                          Lastname = r.R_Lastname
                      });
        }

        [HttpGet("/api/registration/{department}")]
        public ActionResult<RegistrationDto> GetRegistered(string department)
        {
            var result = from r in _db.Registration
                         where r.R_Department.ToUpper() == department.ToUpper()
                         select new RegistrationDto
                         {
                             ID = r.R_ID,
                             Firstname = r.R_Firstname,
                             Lastname = r.R_Lastname,
                             Email = r.R_Email,
                             Date_of_Birth = r.R_Date_of_Birth,
                             Department = r.R_Department,
                             Registration_Date = r.R_Registration_Date,
                             Admitted_Date = r.R_Admitted_Date
                         };
            return Ok(result);
        }

        [HttpGet("/api/admitted/{department}")]
        public ActionResult<RegistrationDto> GetAdmitted(string department)
        {
            var result = from r in _db.Registration
                         where r.R_Department.ToUpper() == department.ToUpper() &&
                               r.R_Admitted_Date != null
                         select new RegistrationDto
                         {
                             ID = r.R_ID,
                             Firstname = r.R_Firstname,
                             Lastname = r.R_Lastname,
                             Email = r.R_Email,
                             Date_of_Birth = r.R_Date_of_Birth,
                             Department = r.R_Department,
                             Registration_Date = r.R_Registration_Date,
                             Admitted_Date = r.R_Admitted_Date
                         };
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<RegistrationDto> WriteRegistration(RegistrationDto registration)
        {
            try
            {
                Registration newRegistration = new Registration
                {
                    R_Firstname = registration.Firstname,
                    R_Lastname = registration.Lastname,
                    R_Email = registration.Email,
                    R_Date_of_Birth = registration.Date_of_Birth,
                    R_DepartmentNavigation = _db.Department.Find(registration.Department) ?? throw new ArgumentException(),
                    R_Registration_Date = DateTime.Now,
                };
                _db.Entry(newRegistration).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _db.SaveChanges();
                return new RegistrationDto
                {
                    ID = newRegistration.R_ID,
                    Firstname = newRegistration.R_Firstname,
                    Lastname = newRegistration.R_Lastname,
                    Email = newRegistration.R_Email,
                    Date_of_Birth = newRegistration.R_Date_of_Birth,
                    Department = newRegistration.R_Department,
                    Registration_Date = newRegistration.R_Registration_Date,
                    Admitted_Date = newRegistration.R_Admitted_Date
                };

            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return BadRequest();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        [HttpPut("admit/{id}")]
        public ActionResult<RegistrationDto> AdmitRegistration(long id)
        {
            Registration registration = _db.Registration.Find(id);
            if (registration == null) { return BadRequest(); }
            registration.R_Admitted_Date = DateTime.Now;
            _db.Entry(registration).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _db.SaveChanges();
            return new RegistrationDto
            {
                ID = registration.R_ID,
                Firstname = registration.R_Firstname,
                Lastname = registration.R_Lastname,
                Email = registration.R_Email,
                Date_of_Birth = registration.R_Date_of_Birth,
                Department = registration.R_Department,
                Registration_Date = registration.R_Registration_Date,
                Admitted_Date = registration.R_Admitted_Date
            };
        }

        [HttpPut("{id}")]
        public ActionResult<RegistrationDto> ChangeRegistration(long id, RegistrationDto newRegistration)
        {
            try
            {
                Registration registration = _db.Registration.Find(id) ?? throw new ArgumentException();
                Department newDepartment = _db.Department.Find(newRegistration.Department) ?? throw new ArgumentException();

                if (registration.R_Admitted_Date != null && registration.R_Department != newRegistration.Department)
                { throw new ArgumentException(); }

                registration.R_Firstname = newRegistration.Firstname;
                registration.R_Lastname = newRegistration.Lastname;
                registration.R_Email = newRegistration.Email;
                registration.R_Date_of_Birth = newRegistration.Date_of_Birth;
                registration.R_DepartmentNavigation = newDepartment;
                _db.Entry(registration).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _db.SaveChanges();
                return new RegistrationDto
                {
                    ID = registration.R_ID,
                    Firstname = registration.R_Firstname,
                    Lastname = registration.R_Lastname,
                    Email = registration.R_Email,
                    Date_of_Birth = registration.R_Date_of_Birth,
                    Department = registration.R_Department,
                    Registration_Date = registration.R_Registration_Date,
                    Admitted_Date = registration.R_Admitted_Date
                };
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return BadRequest();
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult RemoveRegistration(long id)
        {
            Registration registration = _db.Registration.Find(id);
            if (registration == null) { return BadRequest(); }
            if (registration.R_Admitted_Date != null) { return BadRequest(); }
            _db.Entry(registration).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            _db.SaveChanges();
            return Ok();
        }
    }
}