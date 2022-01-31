using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Infrastructure;
using StoreManager.Application.Model;
using System;
using System.Linq;

namespace StoreManager.Webapp.Pages.Stores
{
    public class DetailsModel : PageModel
    {
        private readonly StoreContext _db;

        public DetailsModel(StoreContext db)
        {
            _db = db;
        }

        public Store Store { get; private set; } = default!;
        public IActionResult OnGet(Guid guid)
        {
            // SELECT * FROM Stores INNER JOIN Offers ON (...)
            // INNER JOIN Product ON (...)
            var store = _db.Stores
                .Include(s => s.Offers)
                .ThenInclude(o => o.Product)
                .FirstOrDefault(s => s.Guid == guid);
            if (store == null)
            {
                return RedirectToPage("/Stores/Index");
            }
            Store = store;
            return Page();
        }
    }
}
