using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Infrastructure;
using StoreManager.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreManager.Webapp.Pages.Stores
{
    public class IndexModel : PageModel
    {
        public record StoreWithOfferCount(
            Guid Guid,
            string Name,
            int OffersCount,
            DateTime? CloseDate,
            string? Url);

        private readonly StoreContext _db;
        public List<StoreWithOfferCount> Stores { get; private set; } = new();
        public IndexModel(StoreContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            Stores = _db.Stores
                .OrderBy(s=>s.Name)
                .Select(s => new StoreWithOfferCount(
                    s.Guid,
                    s.Name,
                    s.Offers.Count(),
                    s.CloseDate,
                    s.Url))
                .ToList();
        }
    }
}
