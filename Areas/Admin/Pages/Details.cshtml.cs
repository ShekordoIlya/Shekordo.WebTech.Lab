using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shekordo.Domain.Entities;
using Shekordo.UI.Services;

namespace Shekordo.UI.Areas.Admin.Pages;

[Authorize(Policy = "admin")]
public class DetailsModel(IProductService productService) : PageModel
{
    public Dish Dish { get; set; } = default!;

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
        return Page();
    }
}
