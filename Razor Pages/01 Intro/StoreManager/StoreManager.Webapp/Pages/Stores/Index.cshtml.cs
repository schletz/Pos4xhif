using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreManager.Application.Infrastructure;
using StoreManager.Application.Model;
using System.Collections.Generic;
using System.Linq;

namespace StoreManager.Webapp.Pages.Stores
{
    public class IndexModel : PageModel
    {
        private readonly StoreContext _db;
        public List<Store> Stores { get; private set; } = new();
        public IndexModel(StoreContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            Stores = _db.Stores.OrderBy(s => s.Name).ToList();
        }
    }
}
