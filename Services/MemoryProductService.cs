using Shekordo.Domain.Entities;

namespace Shekordo.UI.Services;

public class MemoryProductService : IProductService
{
    private readonly List<Dish> _dishes;
    private readonly List<Category> _categories;

    public MemoryProductService(ICategoryService categoryService)
    {
        var categoryResponse = categoryService.GetCategoryListAsync().Result;
        _categories = categoryResponse.Data ?? new List<Category>();

        _dishes = new List<Dish>
        {
            new()
            {
                Id = 1,
                Name = "Суп харчо",
                Description = "Острый, грузинский суп",
                Calories = 250,
                Image = "/images/harcho.jpg",
                CategoryId = _categories.FirstOrDefault(c => c.NormalizedName == "soups")?.Id ?? 0
            },
            new()
            {
                Id = 2,
                Name = "Борщ",
                Description = "Красный борщ со сметаной",
                Calories = 300,
                Image = "/images/borsch.jpg",
                CategoryId = _categories.FirstOrDefault(c => c.NormalizedName == "soups")?.Id ?? 0
            },
            new()
            {
                Id = 3,
                Name = "Цезарь",
                Description = "Салат с курицей и соусом",
                Calories = 400,
                Image = "/images/cesar.jpg",
                CategoryId = _categories.FirstOrDefault(c => c.NormalizedName == "salads")?.Id ?? 0
            },
            new()
            {
                Id = 4,
                Name = "Греческий",
                Description = "Салат с фетой и оливками",
                Calories = 350,
                Image = "/images/greek.jpg",
                CategoryId = _categories.FirstOrDefault(c => c.NormalizedName == "salads")?.Id ?? 0
            },
            new()
            {
                Id = 5,
                Name = "Морс",
                Description = "Клюквенный морс",
                Calories = 120,
                Image = "/images/mors.jpg",
                CategoryId = _categories.FirstOrDefault(c => c.NormalizedName == "drinks")?.Id ?? 0
            },
            new()
            {
                Id = 6,
                Name = "Чай",
                Description = "Зелёный чай",
                Calories = 0,
                Image = "/images/tea.jpg",
                CategoryId = _categories.FirstOrDefault(c => c.NormalizedName == "drinks")?.Id ?? 0
            }
        };
    }

    public Task<ResponseData<List<Dish>>> GetProductListAsync(string? categoryNormalizedName)
    {
        var result = new ResponseData<List<Dish>>();

        int? categoryId = null;
        if (!string.IsNullOrEmpty(categoryNormalizedName))
        {
            categoryId = _categories
                .FirstOrDefault(c => c.NormalizedName == categoryNormalizedName)?.Id;
        }

        var data = _dishes
            .Where(d => categoryId == null || d.CategoryId == categoryId)
            .ToList();

        result.Data = data;
        result.Success = true;

        if (data.Count == 0)
        {
            result.Success = false;
            result.ErrorMessage = "Нет блюд в выбранной категории";
        }

        return Task.FromResult(result);
    }
}