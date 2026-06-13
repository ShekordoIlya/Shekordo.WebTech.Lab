using System.Net.Http.Json;
using System.Text.Json;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.UI.Services;

public class ApiCategoryService(HttpClient httpClient) : ICategoryService
{
    public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
    {
        var result = await httpClient.GetAsync(httpClient.BaseAddress);
        if (result.IsSuccessStatusCode)
        {
            return await result.Content.ReadFromJsonAsync<ResponseData<List<Category>>>()
                   ?? new ResponseData<List<Category>> { Success = false, ErrorMessage = "Пустой ответ API" };
        }

        return new ResponseData<List<Category>>
        {
            Success = false,
            ErrorMessage = "Ошибка чтения API"
        };
    }
}
