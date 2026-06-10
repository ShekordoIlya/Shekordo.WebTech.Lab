using Shekordo.Domain.Entities;

namespace Shekordo.UI.Services;

public class MemoryCategoryService : ICategoryService
{
    public Task<ResponseData<List<Category>>> GetCategoryListAsync()
    {
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Супы", NormalizedName = "soups" },
            new() { Id = 2, Name = "Салаты", NormalizedName = "salads" },
            new() { Id = 3, Name = "Напитки", NormalizedName = "drinks" },
            new() { Id = 4, Name = "Горячее", NormalizedName = "hots" }
        };

        var result = new ResponseData<List<Category>>
        {
            Data = categories,
            Success = true
        };

        return Task.FromResult(result);
    }
}