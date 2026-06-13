using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shekordo.Domain.Entities;
using Shekordo.UI.Services;

namespace Shekordo.UI.Areas.Admin.Pages;

[Authorize(Policy = "admin")]
public class IndexModel(IProductService productService) : PageModel
{
    public List<Dish> Dish { get; set; } = [];
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;

    public async Task OnGetAsync(int pageNo = 1)
    {
        var response = await productService.GetProductListAsync(null, pageNo);
        if (response.Success && response.Data != null)
        {
            Dish = response.Data.Items;
            CurrentPage = response.Data.CurrentPage;
            TotalPages = response.Data.TotalPages;
        }
    }
}
