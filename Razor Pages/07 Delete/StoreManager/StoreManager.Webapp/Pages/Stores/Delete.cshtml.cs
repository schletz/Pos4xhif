using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StoreManager.Application.Infrastructure.Repositories;
using StoreManager.Application.Model;
using System;

namespace StoreManager.Webapp.Pages.Stores
{


    public class DeleteModel : PageModel
    {
        private readonly StoreRepository _stores;
        public DeleteModel(StoreRepository stores)
        {
            _stores = stores;
        }

        [TempData]
        public string? Message { get; set; }
        public Store Store { get; set; } = default!;
        public IActionResult OnPostCancel() => RedirectToPage("/Stores/Index");
        public IActionResult OnPostDelete(Guid guid)
        {
            var store = _stores.FindByGuid(guid);
            if (store is null)
            {
                return RedirectToPage("/Stores/Index");
            }
            var (success, message) = _stores.Delete(store);
            if (!success) { Message = message; }
            return RedirectToPage("/Stores/Index");
        }
        public IActionResult OnGet(Guid guid)
        {
            var store = _stores.FindByGuid(guid);
            if (store is null)
            {
                return RedirectToPage("/Stores/Index");
            }
            Store = store;
            return Page();
        }
    }
}
