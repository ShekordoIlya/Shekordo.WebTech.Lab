using Microsoft.AspNetCore.Mvc;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;
using Shekordo.UI.Services;

namespace Shekordo.UI.Controllers;

public class ProductController(ICategoryService categoryService, IProductService productService) : Controller
{
    [Route("Catalog")]
    [Route("Catalog/{category}")]
    public async Task<IActionResult> Index(string? category, int pageNo = 1)
    {
        var categoriesResponse = await categoryService.GetCategoryListAsync();
        if (!categoriesResponse.Success)
        {
            return NotFound(categoriesResponse.ErrorMessage);
        }

        ViewData["categories"] = categoriesResponse.Data;

        var currentCategory = string.IsNullOrEmpty(category)
            ? "Все"
            : categoriesResponse.Data?.FirstOrDefault(c => c.NormalizedName == category)?.Name ?? "Все";
        ViewData["currentCategory"] = currentCategory;

        var productsResponse = await productService.GetProductListAsync(category, pageNo);
        if (!productsResponse.Success)
        {
            ViewData["Error"] = productsResponse.ErrorMessage;
        }

        return View(productsResponse.Data ?? new ListModel<Dish>());
    }
}
