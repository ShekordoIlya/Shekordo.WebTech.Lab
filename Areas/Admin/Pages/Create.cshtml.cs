using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shekordo.Domain.Entities;
using Shekordo.UI.Services;

namespace Shekordo.UI.Areas.Admin.Pages;

[Authorize(Policy = "admin")]
public class CreateModel(ICategoryService categoryService, IProductService productService) : PageModel
{
    [BindProperty]
    public Dish Dish { get; set; } = default!;

    [BindProperty]
    public IFormFile? Image { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var categoryListData = await categoryService.GetCategoryListAsync();
        ViewData["CategoryId"] = new SelectList(categoryListData.Data, "Id", "Name");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        await productService.CreateProductAsync(Dish, Image);
        return RedirectToPage("./Index");
    }
}
