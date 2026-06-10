using Microsoft.AspNetCore.Mvc;
using Shekordo.Domain.Entities;
using Shekordo.UI.Services;

namespace Shekordo.UI.Controllers;

public class ProductController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly IProductService _productService;

    public ProductController(ICategoryService categoryService, IProductService productService)
    {
        _categoryService = categoryService;
        _productService = productService;
    }

    public async Task<IActionResult> Index(string? category)
    {
        var categoriesResponse = await _categoryService.GetCategoryListAsync();
        if (categoriesResponse.Success && categoriesResponse.Data != null)
        {
            ViewBag.Categories = categoriesResponse.Data;
        }

        var currentCategory = string.IsNullOrEmpty(category) ? "Все" :
            categoriesResponse.Data?.FirstOrDefault(c => c.NormalizedName == category)?.Name ?? "Все";
        ViewBag.CurrentCategory = currentCategory;

        var productsResponse = await _productService.GetProductListAsync(category);

        if (!productsResponse.Success)
        {
            ViewBag.Error = productsResponse.ErrorMessage;
        }

        return View(productsResponse.Data ?? new List<Dish>());
    }
}