using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Infrastructure.Repositories;
using StoreManager.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreManager.Webapp.Pages.Products
{
    [Authorize(Roles = "Admin,Owner")]
    public class IndexModel : PageModel
    {
        private readonly ProductRepository _products;
        private readonly ProductCategoryRepository _categories;

        public IndexModel(ProductRepository products, ProductCategoryRepository categories)
        {
            _products = products;
            _categories = categories;
        }

        /// <summary>
        /// GET /Products/Index?handler=Categories
        /// </summary>
        public IActionResult OnGetCategories()
        {
            return new JsonResult(_categories.Set
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.Guid,
                    c.Name,
                    c.NameEn,
                }));
        }
        /// <summary>
        /// GET /Products/Index?handler=Products&categoryGuid=(guid)
        /// </summary>
        public IActionResult OnGetProducts(Guid categoryGuid)
        {
            var products = _products.Set
                .Where(p=>p.ProductCategory.Guid == categoryGuid)
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Guid,
                    p.Ean,
                    p.Name,
                    p.RecommendedPrice,
                    p.AvailableFrom
                });
            return new JsonResult(products);
        }
    }
}
