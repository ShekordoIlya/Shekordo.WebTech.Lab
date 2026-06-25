using Microsoft.AspNetCore.Mvc;
using Shekordo.Domain.Entities;
using Shekordo.UI.Services;

namespace Shekordo.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        [Route("Catalog")]
        [Route("Catalog/{category}")]
        public async Task<IActionResult> Index(string? category, int pageNo = 1)
        {
            // Получить список категорий
            var categoriesResponse = await _categoryService.GetCategoryListAsync();

            // Если список не получен, вернуть код 404
            if (!categoriesResponse.Success)
                return NotFound(categoriesResponse.ErrorMessage);

            // Передать список категорий во ViewData
            ViewData["categories"] = categoriesResponse.Data;

            // Передать во ViewData имя текущей категории
            var currentCategory = category == null
                ? "Все"
                : categoriesResponse.Data.FirstOrDefault(c => c.NormalizedName == category)?.Name;
            ViewData["currentCategory"] = currentCategory;

            var productResponse = await _productService.GetProductListAsync(category, pageNo);
            if (!productResponse.Success)
                ViewData["Error"] = productResponse.ErrorMessage;

            // Передаём весь объект ListModel, а не только Items
            return View(productResponse.Data);
        }
    }
}