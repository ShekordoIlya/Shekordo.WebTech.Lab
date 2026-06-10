using Shekordo.Domain.Entities;

namespace Shekordo.UI.Services;

public interface ICategoryService
{
    Task<ResponseData<List<Category>>> GetCategoryListAsync();
}