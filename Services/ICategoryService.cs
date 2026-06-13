using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.UI.Services;

public interface ICategoryService
{
    Task<ResponseData<List<Category>>> GetCategoryListAsync();
}