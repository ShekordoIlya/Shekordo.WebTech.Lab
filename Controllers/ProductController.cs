using Microsoft.AspNetCore.Mvc;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;
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

    [Route("Catalog")]
    [Route("Catalog/{category}")]

    public async Task<IActionResult> Index(string? category, int pageNo = 1)
    {
        var categoriesResponse = await _categoryService.GetCategoryListAsync();
        if (categoriesResponse.Success && categoriesResponse.Data != null)
        {
            ViewBag.Categories = categoriesResponse.Data;
        }

        var currentCategory = string.IsNullOrEmpty(category) ? "Все" :
            categoriesResponse.Data?.FirstOrDefault(c => c.NormalizedName == category)?.Name ?? "Все";
        ViewBag.CurrentCategory = currentCategory;

        var productsResponse = await _productService.GetProductListAsync(category, pageNo);

        if (!productsResponse.Success)
        {
            ViewBag.Error = productsResponse.ErrorMessage;
        }

        return View(productsResponse.Data ?? new ListModel<Dish>());
    }
}