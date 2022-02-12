using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Dto;
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
        private readonly IMapper _mapper;
        public IndexModel(ProductRepository products, ProductCategoryRepository categories, IMapper mapper)
        {
            _products = products;
            _categories = categories;
            _mapper = mapper;
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
                .Where(p => p.ProductCategory.Guid == categoryGuid)
                .OrderBy(p => p.Name)
                .ProjectTo<ProductDto>(_mapper.ConfigurationProvider);
            return new JsonResult(products);
        }

        public IActionResult OnPostProduct(ProductDto productDto) => UpsertProduct(productDto);
        public IActionResult OnPutProduct(ProductDto productDto) => UpsertProduct(productDto);
        public IActionResult OnDeleteProduct(Guid productGuid)
        {
            var (success, message) = _products.DeleteByGuid(productGuid);
            if (!success) { return BadRequest(message); }
            return new NoContentResult();
        }
        private IActionResult UpsertProduct(ProductDto productDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var category = _categories.Set.FirstOrDefault(c => c.Guid == productDto.ProductCategoryGuid);
            if (category is null) { return BadRequest("Kategorie nicht gefunden."); }

            var product = _mapper.Map<ProductDto, Product>(
                productDto,
                opt => opt.AfterMap((dto, entity) => entity.ProductCategory = category));

            bool isInsert = productDto.Guid == default;
            var (success, message) = isInsert
                ? _products.Insert(product)
                : _products.Update(product);
            if (!success) { return BadRequest(message); }
            return isInsert
                ? new CreatedResult("/Product/Index", _mapper.Map<ProductDto>(product))
                : new NoContentResult();
        }
    }
}
