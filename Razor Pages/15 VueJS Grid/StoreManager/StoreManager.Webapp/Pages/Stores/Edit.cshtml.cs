using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Dto;
using StoreManager.Application.Infrastructure;
using StoreManager.Application.Infrastructure.Repositories;
using StoreManager.Application.Model;
using System;
using System.Linq;

namespace StoreManager.Webapp.Pages.Stores
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly StoreRepository _stores;
        private readonly IMapper _mapper;

        public EditModel(IMapper mapper, StoreRepository stores)
        {
            _mapper = mapper;
            _stores = stores;
        }
        [BindProperty]
        public StoreDto Store { get; set; } = null!;

        public IActionResult OnPost(Guid guid)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var store = _stores.FindByGuid(guid);
            if (store is null)
            {
                return RedirectToPage("/Stores/Index");
            }
            _mapper.Map(Store, store);
            var (success, message) = _stores.Update(store);
            if (!success)
            {
                ModelState.AddModelError("", message);
                return Page();
            }
            return RedirectToPage("/Stores/Index");
        }
        public IActionResult OnGet(Guid guid)
        {
            var store = _stores.FindByGuid(guid);
            if (store is null)
            {
                return RedirectToPage("/Stores/Index");
            }
            Store = _mapper.Map<StoreDto>(store);
            return Page();
        }
    }
}
