using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Shekordo.Domain.Entities;
using Shekordo.UI.Services;

namespace Shekordo.UI.Areas.Admin.Pages;

[Authorize(Policy = "admin")]
public class EditModel(ICategoryService categoryService, IProductService productService) : PageModel
{
    [BindProperty]
    public Dish Dish { get; set; } = default!;

    [BindProperty]
    public IFormFile? Image { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var response = await productService.GetProductByIdAsync(id.Value);
        if (!response.Success || response.Data == null)
        {
            return NotFound();
        }

        Dish = response.Data;
        var categoryListData = await categoryService.GetCategoryListAsync();
        ViewData["CategoryId"] = new SelectList(categoryListData.Data, "Id", "Name", Dish.CategoryId);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await productService.UpdateProductAsync(Dish.Id, Dish, Image);
        return RedirectToPage("./Index");
    }
}
