using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Infrastructure.Repositories;
using StoreManager.Application.Model;
using System.Collections.Generic;
using System.Linq;

namespace StoreManager.Webapp.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ProductRepository _products;

        public IndexModel(ProductRepository products)
        {
            _products = products;
        }

        public IEnumerable<Product> Products =>
            _products.Set
            .Include(p => p.ProductCategory)
            .OrderBy(p => p.Ean);
        public void OnGet()
        {
        }
    }
}
