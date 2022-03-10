using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProcessDemo.Model;
using System.Collections.Generic;
using System.Linq;

namespace ProcessDemo.Pages
{
    public class JobsModel : PageModel
    {
        private readonly PingContext _db;
        public List<Job> Jobs = new();
        public JobsModel(PingContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            Jobs = _db.Jobs.OrderByDescending(j => j.StartTime).Include(d => d.PingResults).ToList();
        }
    }
}
