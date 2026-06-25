using System.Net.Http.Json;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.UI.Services
{
    public class ApiCategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;

        public ApiCategoryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            try
            {
                var result = await _httpClient.GetAsync(_httpClient.BaseAddress);

                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadFromJsonAsync<ResponseData<List<Category>>>();
                    return response ?? new ResponseData<List<Category>>
                    {
                        Success = false,
                        ErrorMessage = "Пустой ответ от API"
                    };
                }

                return new ResponseData<List<Category>>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка чтения API: {result.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<List<Category>>
                {
                    Success = false,
                    ErrorMessage = $"Исключение: {ex.Message}"
                };
            }
        }
    }
}