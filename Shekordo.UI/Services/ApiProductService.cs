using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.UI.Services
{
    public class ApiProductService : IProductService
    {
        private readonly HttpClient _httpClient;

        public ApiProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ResponseData<ListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            try
            {
                // Формируем query string
                var queryData = new Dictionary<string, string>
                {
                    { "pageNo", pageNo.ToString() }
                };

                if (!string.IsNullOrEmpty(categoryNormalizedName))
                {
                    queryData.Add("category", categoryNormalizedName);
                }

                var query = QueryString.Create(queryData);
                var url = _httpClient.BaseAddress + query.Value;

                var result = await _httpClient.GetAsync(url);

                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadFromJsonAsync<ResponseData<ListModel<Dish>>>();
                    return response ?? new ResponseData<ListModel<Dish>>
                    {
                        Success = false,
                        ErrorMessage = "Пустой ответ от API"
                    };
                }

                return new ResponseData<ListModel<Dish>>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка чтения API: {result.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<ListModel<Dish>>
                {
                    Success = false,
                    ErrorMessage = $"Исключение: {ex.Message}"
                };
            }
        }

        public async Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            try
            {
                var result = await _httpClient.GetAsync($"{_httpClient.BaseAddress}{id}");

                if (result.IsSuccessStatusCode)
                {
                    var response = await result.Content.ReadFromJsonAsync<ResponseData<Dish>>();
                    return response ?? new ResponseData<Dish>
                    {
                        Success = false,
                        ErrorMessage = "Пустой ответ от API"
                    };
                }

                return new ResponseData<Dish>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка чтения API: {result.StatusCode}"
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<Dish>
                {
                    Success = false,
                    ErrorMessage = $"Исключение: {ex.Message}"
                };
            }
        }

        public Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
        {
            throw new NotImplementedException();
        }

        public Task DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile)
        {
            var responseData = new ResponseData<Dish>();

            try
            {
                // Послать запрос к API для сохранения объекта
                var response = await _httpClient.PostAsJsonAsync("", product);

                if (!response.IsSuccessStatusCode)
                {
                    responseData.Success = false;
                    responseData.ErrorMessage = $"Не удалось создать объект: {response.StatusCode}";
                    return responseData;
                }

                // Если файл изображения передан клиентом
                if (formFile != null && formFile.Length > 0)
                {
                    // получить созданный объект из ответа Api-сервиса
                    var dish = await response.Content.ReadFromJsonAsync<Dish>();

                    if (dish != null)
                    {
                        // создать объект запроса
                        var request = new HttpRequestMessage
                        {
                            Method = HttpMethod.Post,
                            RequestUri = new Uri($"{_httpClient.BaseAddress}{dish.Id}")
                        };

                        // Создать контент типа multipart form-data
                        var content = new MultipartFormDataContent();
                        // создать потоковый контент из переданного файла
                        var streamContent = new StreamContent(formFile.OpenReadStream());
                        // добавить потоковый контент в общий контент по именем "image"
                        content.Add(streamContent, "image", formFile.FileName);
                        // поместить контент в запрос
                        request.Content = content;

                        // послать запрос к Api-сервису
                        response = await _httpClient.SendAsync(request);

                        if (!response.IsSuccessStatusCode)
                        {
                            responseData.Success = false;
                            responseData.ErrorMessage = $"Не удалось сохранить изображение: {response.StatusCode}";
                            return responseData;
                        }
                    }
                }

                responseData.Data = product;
                responseData.Success = true;
            }
            catch (Exception ex)
            {
                responseData.Success = false;
                responseData.ErrorMessage = $"Исключение: {ex.Message}";
            }

            return responseData;
        }
    }
}