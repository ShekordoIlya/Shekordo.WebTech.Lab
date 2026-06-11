using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.UI.Services;

public interface IProductService
{
    Task<ResponseData<ListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1);
}