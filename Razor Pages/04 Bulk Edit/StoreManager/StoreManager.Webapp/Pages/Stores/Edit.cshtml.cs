using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Infrastructure;
using StoreManager.Application.Model;
using StoreManager.Webapp.Dto;
using System;
using System.Linq;

namespace StoreManager.Webapp.Pages.Stores
{
    public class EditModel : PageModel
    {
        private readonly StoreContext _db;
        private readonly IMapper _mapper;

        public EditModel(StoreContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        [BindProperty]
        public StoreDto Store { get; set; } = null!;

        public IActionResult OnPost(Guid guid)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var store = _db.Stores.FirstOrDefault(s => s.Guid == guid);
            if (store is null)
            {
                return RedirectToPage("/Stores/Index");
            }
            _mapper.Map(Store, store);
            _db.Entry(store).State = EntityState.Modified;
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Fehler beim Schreiben in die Datenbank");
                return Page();
            }
            return RedirectToPage("/Stores/Index");
        }
        public IActionResult OnGet(Guid guid)
        {
            var store = _db.Stores.FirstOrDefault(s => s.Guid == guid);
            if (store is null)
            {
                return RedirectToPage("/Stores/Index");
            }
            Store = _mapper.Map<StoreDto>(store);
            return Page();
        }
    }
}
