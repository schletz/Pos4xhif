using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Infrastructure.Repositories;
using StoreManager.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreManager.Webapp.Pages.Stores
{
    public class IndexModel : PageModel
    {
        private readonly StoreRepository _stores;

        public IndexModel(StoreRepository stores)
        {
            _stores = stores;
        }

        public IReadOnlyList<StoreRepository.StoreWithOffersCount> Stores { get; private set; } = new List<StoreRepository.StoreWithOffersCount>();

        public void OnGet()
        {
            Stores = _stores.GetStoresWithOffersCount();
        }
    }
}
