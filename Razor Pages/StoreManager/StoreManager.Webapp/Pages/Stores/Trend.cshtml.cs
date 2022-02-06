using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Extensions;
using StoreManager.Application.Infrastructure.Repositories;
using System;
using System.Linq;

namespace StoreManager.Webapp.Pages.Stores
{
    public class TrendModel : PageModel
    {
        private readonly StoreRepository _stores;
        private readonly PriceTrendRepository _trends;

        public TrendModel(StoreRepository stores, PriceTrendRepository trends)
        {
            _stores = stores;
            _trends = trends;
        }

        public IActionResult OnGetStores()
        {
            var stores = _stores.Set
                .OrderBy(s => s.Name)
                .Select(s => new
                {
                    s.Guid,
                    s.Name,
                    Offers = s.Offers
                        .OrderBy(o => o.Product.Name)
                        .Select(o => new
                        {
                            o.Guid,
                            ProductName = o.Product.Name,
                        })
                });
            return new JsonResult(stores);
        }

        public IActionResult OnGetTrenddata([FromQuery] Guid offerGuid)
        {
            var trenddata = _trends.Set
                .Where(t => t.Offer.Guid == offerGuid)
                .OrderBy(t => t.Date)
                .Select(t => new decimal[] { t.Date.ToJavascriptTimestamp(), t.Price });
            return new JsonResult(trenddata);
        }
    }
}
