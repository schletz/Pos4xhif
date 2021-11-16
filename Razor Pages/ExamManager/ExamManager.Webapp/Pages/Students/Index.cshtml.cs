using ExamManager.App.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExamManager.Webapp.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly ExamContext _db;
        public IReadOnlyList<Student> Students { get; private set; }
            = new List<Student>();
        public IndexModel(ExamContext db)
        {
            _db = db;
        }
        /// <summary>
        /// Page handler for GET requests
        /// </summary>
        public void OnGet()
        {
            // To prevent lazy loading (n+1 problem)
            // we can enforce a join by explicit loading.
            // Now we can access SchoolClass properties
            // without additional queries.
            Students = _db
                .Students
                .Include(s => s.SchoolClass)
                .OrderBy(s => s.Lastname).ThenBy(s => s.Firstname)
                .ToList();
        }
    }
}
