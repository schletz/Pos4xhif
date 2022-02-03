using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Infrastructure;
using StoreManager.Application.Model;
using StoreManager.Webapp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Filters;

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

        public Store Store { get; private set; } = default!;

        [FromRoute]
        public Guid Guid { get; set; }

        [BindProperty]
        public Dictionary<Guid, OfferDtoBase> Offers { get; set; } = new();
        
        public IActionResult OnPost(Guid guid, Guid offerGuid)
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
            // Braucht ein Mapping in Dto/MappingProfile.cs
            _mapper.Map(Offers[offerGuid], offer);
            offer.LastUpdate = DateTime.UtcNow;
            _db.Entry(offer).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToPage();
        }
        public IActionResult OnGet(Guid guid)
        {
            // Braucht ein Mapping in Dto/MappingProfile.cs        
            Offers = _mapper.Map<IEnumerable<OfferDto>>(Store.Offers)
                .ToDictionary(o => o.Guid, o => (OfferDtoBase)o);
            return Page();
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var store = _db.Stores
                .Include(s => s.Offers)
                .ThenInclude(o => o.Product)
                .FirstOrDefault(s => s.Guid == Guid);
            if (store == null)
            {
                context.Result = RedirectToPage("/Stores/Index");
                return;
            }
            Store = store;
        }
    }
}
