using Shekordo.Domain.Entities;

namespace Shekordo.UI.Services;

public interface IProductService
{
    Task<ResponseData<List<Dish>>> GetProductListAsync(string? categoryNormalizedName);
}