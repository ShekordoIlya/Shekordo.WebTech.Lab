using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.UI.Services;

public class MemoryCategoryService : ICategoryService
{
    public Task<ResponseData<List<Category>>> GetCategoryListAsync()
    {
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Стартеры", NormalizedName = "starters" },
            new() { Id = 2, Name = "Салаты", NormalizedName = "salads" },
            new() { Id = 3, Name = "Супы", NormalizedName = "soups" },
            new() { Id = 4, Name = "Основные блюда", NormalizedName = "main-dishes" },
            new() { Id = 5, Name = "Напитки", NormalizedName = "drinks" },
            new() { Id = 6, Name = "Десерты", NormalizedName = "desserts" }
        };

        return Task.FromResult(new ResponseData<List<Category>> { Data = categories });
    }
}
