using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plf4ahif20191118.Model;
using Plf4ahif20191118.Dto;

namespace Plf4ahif20191118.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchoolratingController : ControllerBase
    {
        private readonly RatingContext _db;
        public SchoolratingController(RatingContext context)
        {
            _db = context;
        }
        [HttpGet]
        public ActionResult<SchoolDto> Get()
        {
            return Ok(from s in _db.School
                      select new SchoolDto()
                      {
                          Nr = s.S_Nr,
                          Name = s.S_Name,
                          AvgRating = Math.Round(_db.School_Rating.Where(sr => sr.SR_User_School == s.S_Nr).Average(sr => sr.SR_Value), 1)
                      }
                );
        }

        [HttpPost]
        public ActionResult<RatingDto> Post(RatingDto rating)
        {
            User user = _db.User.FirstOrDefault(u => u.U_Phonenr == rating.PhoneNr && u.U_School == rating.School);
            if (user == null)
            {
                return BadRequest();
            }
            if (_db.School_Rating.Any(sr => sr.SR_User_Phonenr == rating.PhoneNr && sr.SR_User_School == rating.School))
            {
                return BadRequest();
            }
            School_Rating newRating = new School_Rating
            {
                SR_Date = DateTime.Now.Date,
                SR_User_Navigation = user,
                SR_Value = rating.Value
            };
            _db.Entry(newRating).State = Microsoft.EntityFrameworkCore.EntityState.Added;
            _db.SaveChanges();
            rating.Id = newRating.SR_ID;
            return Ok(rating);
        }

        [HttpPost("/api/register")]
        public ActionResult<UserDto> RegisterUser(UserDto user)
        {
            User newUser = new User()
            {
                U_Phonenr = user.PhoneNr,
                U_School = user.School
            };
            try
            {
                _db.Entry(newUser).State = Microsoft.EntityFrameworkCore.EntityState.Added;
                _db.SaveChanges();
                return Ok(user);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                return BadRequest();
            }
        }

        [HttpPut("{ratingid}")]
        public ActionResult<RatingDto> ChangeRating(long ratingid, RatingDto newRating)
        {
            DateTime today = DateTime.Now.Date;
            School_Rating rating = _db.School_Rating.FirstOrDefault(sr => sr.SR_ID == ratingid && sr.SR_Date < today);
            if (rating == null)
            { return BadRequest(); }

            rating.SR_Value = newRating.Id;
            rating.SR_Date = today;
            rating.SR_Value = newRating.Value;
            _db.Entry(rating).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _db.SaveChanges();
            return new RatingDto
            {
                Date = rating.SR_Date,
                Id = rating.SR_ID,
                PhoneNr = rating.SR_User_Phonenr,
                School = rating.SR_User_School,
                Value = rating.SR_Value
            };
        }

        [HttpDelete("{ratingid}")]
        public ActionResult<RatingDto> DeleteRating(long ratingid)
        {
            School_Rating rating = _db.School_Rating.Find(ratingid);
            if (rating == null)
            { return BadRequest(); }
            _db.Entry(rating).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            _db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Testroute zum Prüfen ob alles funktioniert.
        /// </summary>
        /// <returns></returns>
        [HttpGet("test")]
        public ActionResult<SchoolDto> GetSchoolTest()
        {
            return Ok(from s in _db.School
                      select new SchoolDto
                      {
                          Nr = s.S_Nr,
                          Name = s.S_Name
                      });
        }

    }
}
