using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Dto;
using StoreManager.Application.Infrastructure;
using StoreManager.Application.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StoreManager.Webapp.Pages.Stores
{
    public class DetailsModel : PageModel
    {
        private readonly StoreContext _db;
        private readonly IMapper _mapper;

        public DetailsModel(StoreContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [FromRoute]
        public Guid Guid { get; set; }

        public OfferDto NewOffer { get; set; } = default!;
        public Store Store { get; private set; } = default!;
        public IReadOnlyList<Offer> Offers { get; private set; } = new List<Offer>();
        public Dictionary<Guid, OfferDto> EditOffers { get; set; } = new();
        public IEnumerable<SelectListItem> ProductSelectList =>
            _db.Products.OrderBy(p => p.Name).Select(p => new SelectListItem(p.Name, p.Guid.ToString()));

        public IActionResult OnPostEditOffer(Guid guid, Guid offerGuid, Dictionary<Guid, OfferDto> editOffers)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var offer = _db.Offers.FirstOrDefault(o => o.Guid == offerGuid);
            if (offer is null)
            {
                return RedirectToPage();
            }
            _mapper.Map(editOffers[offerGuid], offer);
            offer.LastUpdate = DateTime.UtcNow;
            _db.Entry(offer).State = EntityState.Modified;
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Fehler beim Schreiben in die Datenbank.");
                return Page();
            }
            return RedirectToPage();
        }
        public IActionResult OnPostNewOffer(Guid guid, OfferDto newOffer)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                var offer = _mapper.Map<Offer>(newOffer);
                offer.Product = _db.Products.FirstOrDefault(p => p.Guid == newOffer.ProductGuid)
                    ?? throw new ApplicationException("Ungültiges Produkt.");
                offer.Store = _db.Stores.FirstOrDefault(s => s.Guid == guid)
                    ?? throw new ApplicationException("Ungültiger Store.");
                offer.LastUpdate = DateTime.UtcNow;
                _db.Offers.Add(offer);
                _db.SaveChanges();
            }
            catch (ApplicationException e)
            {
                ModelState.AddModelError("", e.Message);
                return Page();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Fehler beim Schreiben in die Datenbank.");
                return Page();
            }
            return RedirectToPage();
        }
        public IActionResult OnGet(Guid guid)
        {
            return Page();
        }

        // Filter: Wird vor jedem Handler ausgeführt
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            // SELECT * FROM Stores INNER JOIN Offers ON (...)
            // INNER JOIN Product ON (...)
            var store = _db.Stores
                .FirstOrDefault(s => s.Guid == Guid);
            if (store is null)
            {
                context.Result = RedirectToPage("/Stores/Index");
                return;
            }
            Store = store;
            Offers = _db.Offers.Include(o => o.Product).Where(o => o.Store.Guid == Guid).ToList();
            EditOffers = _db.Offers.Where(o => o.Store.Guid == Guid)
                .ProjectTo<OfferDto>(_mapper.ConfigurationProvider)
                .ToDictionary(o => o.Guid, o => o);
        }
    }
}
