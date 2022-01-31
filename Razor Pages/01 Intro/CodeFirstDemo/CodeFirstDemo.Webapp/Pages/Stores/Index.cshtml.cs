using CodeFirstDemo.Application.Infrastructure;
using CodeFirstDemo.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeFirstDemo.Webapp.Pages.Stores
{
    public class IndexModel : PageModel
    {
        private readonly StoreContext _db;

        public IndexModel(StoreContext db)
        {
            _db = db;
        }

        public List<Store> Stores { get; private set; } = new();
        public string Time { get; private set; } = String.Empty;
        public void OnGet()
        {
            Stores = _db.Stores.ToList();
            Time = DateTime.Now.ToString("HH:mm");
        }
    }
}
