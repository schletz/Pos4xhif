using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Infrastructure.Repositories;
using StoreManager.Application.Model;
using StoreManager.Webapp.Dto;
using StoreManager.Webapp.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StoreManager.Webapp.Pages.Stores
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly StoreRepository _stores;
        private readonly OfferRepository _offers;
        private readonly ProductRepository _products;
        private readonly IMapper _mapper;
        private readonly AuthService _authService;

        public DetailsModel(IMapper mapper, StoreRepository stores, OfferRepository offers, ProductRepository products, AuthService authService)
        {
            _mapper = mapper;
            _stores = stores;
            _offers = offers;
            _products = products;
            _authService = authService;
        }

        [FromRoute]
        public Guid Guid { get; set; }

        public OfferDto NewOffer { get; set; } = default!;
        public Store Store { get; private set; } = default!;
        public IReadOnlyList<Offer> Offers { get; private set; } = new List<Offer>();
        public Dictionary<Guid, OfferDto> EditOffers { get; set; } = new();
        public Dictionary<Guid, bool> OffersToDelete { get; set; } = new();
        public IEnumerable<SelectListItem> ProductSelectList =>
            _products.Set.OrderBy(p => p.Name).Select(p => new SelectListItem(p.Name, p.Guid.ToString()));

        public IActionResult OnPostEditOffer(Guid guid, Guid offerGuid, Dictionary<Guid, OfferDto> editOffers)
        {
            if (!ModelState.IsValid) { return Page(); }
            var offer = _offers.FindByGuid(offerGuid);
            if (offer is null) { return RedirectToPage(); }
            _mapper.Map(editOffers[offerGuid], offer);
            var (success, message) = _offers.Update(offer);
            if (!success)
            {
                ModelState.AddModelError("", message!);
                return Page();
            }
            return RedirectToPage();
        }
        public IActionResult OnPostNewOffer(Guid guid, OfferDto newOffer)
        {
            if (!ModelState.IsValid) { return Page(); }
            var (success, message) = _offers.Insert(
                price: newOffer.Price, soldOut: newOffer.SoldOut,
                productGuid: newOffer.ProductGuid,
                storeGuid: guid);
            if (!success)
            {
                ModelState.AddModelError("", message!);
                return Page();
            }
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(Guid guid, Dictionary<Guid, bool> offersToDelete)
        {
            var offersDb = _offers.Set.Where(o => o.Store.Guid == guid).ToDictionary(o => o.Guid, o => o);
            var offerGuids = offersToDelete.Where(o => o.Value == true).Select(o => o.Key);

            foreach (var o in offerGuids)
            {
                if (!offersDb.TryGetValue(o, out var offer))
                {
                    continue;
                }
                _offers.Delete(offer);
            }
            return RedirectToPage();
        }
        public IActionResult OnGet(Guid guid)
        {
            return Page();
        }

        /// <summary>
        /// Action filter. Wird VOR jedem Page Handler ausgeführt, aber NACHDEM Model
        /// Binding aktiv wurde.
        /// </summary>
        /// <param name="context"></param>
        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            // SELECT * FROM Stores INNER JOIN Offers ON (...)
            // INNER JOIN Product ON (...)
            var store = _stores.Set
                .Include(s => s.Manager)
                .Include(s => s.Offers)
                .ThenInclude(o => o.Product)
                .FirstOrDefault(s => s.Guid == Guid);
            if (store is null)
            {
                context.Result = RedirectToPage("/Stores/Index");
                return;
            }
            var username = _authService.Username;
            if (!_authService.HasRole("Admin") && username != store.Manager?.Username)
            {
                context.Result = new ForbidResult();
                return;
            }
            Store = store;
            Offers = store.Offers.ToList();
            OffersToDelete = Offers.ToDictionary(o => o.Guid, o => false);
            EditOffers = _offers.Set.Where(o => o.Store.Guid == Guid)
                .ProjectTo<OfferDto>(_mapper.ConfigurationProvider)
                .ToDictionary(o => o.Guid, o => o);
        }
    }
}
