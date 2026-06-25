using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shekordo.Domain.Entities;
using Shekordo.UI.Services;

namespace Shekordo.UI.Areas.Admin.Pages
{
    [Authorize(Policy = "admin")]
    public class CreateModel : PageModel
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public CreateModel(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<IActionResult> OnGet()
        {
            var categoryListData = await _categoryService.GetCategoryListAsync();
            if (categoryListData.Success && categoryListData.Data != null)
            {
                ViewData["CategoryId"] = new SelectList(categoryListData.Data, "Id", "Name");
            }
            return Page();
        }

        [BindProperty]
        public Dish Dish { get; set; } = default!;

        [BindProperty]
        public Microsoft.AspNetCore.Http.IFormFile? Image { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var categoryListData = await _categoryService.GetCategoryListAsync();
                if (categoryListData.Success && categoryListData.Data != null)
                {
                    ViewData["CategoryId"] = new SelectList(categoryListData.Data, "Id", "Name");
                }
                return Page();
            }

            await _productService.CreateProductAsync(Dish, Image);

            return RedirectToPage("./Index");
        }
    }
}