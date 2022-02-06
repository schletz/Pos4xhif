using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Infrastructure.Repositories;
using StoreManager.Application.Model;
using StoreManager.Webapp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreManager.Webapp.Pages.Stores
{
    public class IndexModel : PageModel
    {
        private readonly StoreRepository _stores;
        private readonly AuthService _authService;

        public IndexModel(StoreRepository stores, AuthService authService)
        {
            _stores = stores;
            _authService = authService;
        }

        [TempData]
        public string? Message { get; set; }
        public IReadOnlyList<StoreRepository.StoreWithOffersCount> Stores { get; private set; } = new List<StoreRepository.StoreWithOffersCount>();

        public void OnGet()
        {
            Stores = _stores.GetStoresWithOffersCount();
        }

        public bool CanEditStore(Guid storeGuid) =>
            _authService.IsAdmin
            || Stores.FirstOrDefault(s => s.Guid == storeGuid)?.Manager?.Username == _authService.Username;

    }
}
