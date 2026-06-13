using System.Net.Http.Json;
using System.Text.Json;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.UI.Services;

public class ApiProductService(HttpClient httpClient) : IProductService
{
    public async Task<ResponseData<ListModel<Dish>>> GetProductListAsync(
        string? categoryNormalizedName,
        int pageNo = 1)
    {
        var queryData = new Dictionary<string, string?>
        {
            ["pageNo"] = pageNo.ToString()
        };

        if (!string.IsNullOrEmpty(categoryNormalizedName))
        {
            queryData["category"] = categoryNormalizedName;
        }

        var query = QueryString.Create(queryData);
        var result = await httpClient.GetAsync(query.Value);

        if (result.IsSuccessStatusCode)
        {
            return await result.Content.ReadFromJsonAsync<ResponseData<ListModel<Dish>>>()
                   ?? new ResponseData<ListModel<Dish>> { Success = false, ErrorMessage = "Пустой ответ API" };
        }

        return new ResponseData<ListModel<Dish>>
        {
            Success = false,
            ErrorMessage = "Ошибка чтения API"
        };
    }

    public async Task<ResponseData<Dish>> GetProductByIdAsync(int id)
    {
        var result = await httpClient.GetAsync($"{id}");
        if (result.IsSuccessStatusCode)
        {
            var dish = await result.Content.ReadFromJsonAsync<Dish>();
            return new ResponseData<Dish> { Data = dish, Success = dish != null };
        }

        return new ResponseData<Dish> { Success = false, ErrorMessage = "Объект не найден" };
    }

    public async Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
    {
        product.Id = id;
        var response = await httpClient.PutAsJsonAsync($"{id}", product);
        response.EnsureSuccessStatusCode();

        if (formFile != null)
        {
            await UploadImageAsync(id, formFile);
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile)
    {
        var responseData = new ResponseData<Dish>();
        var response = await httpClient.PostAsJsonAsync(httpClient.BaseAddress, product);

        if (!response.IsSuccessStatusCode)
        {
            responseData.Success = false;
            responseData.ErrorMessage = $"Не удалось создать объект: {response.StatusCode}";
            return responseData;
        }

        var dish = await response.Content.ReadFromJsonAsync<Dish>();
        responseData.Data = dish;

        if (formFile != null && dish != null)
        {
            var imageResponse = await UploadImageAsync(dish.Id, formFile);
            if (!imageResponse.IsSuccessStatusCode)
            {
                responseData.Success = false;
                responseData.ErrorMessage = $"Не удалось сохранить изображение: {imageResponse.StatusCode}";
            }
        }

        return responseData;
    }

    private async Task<HttpResponseMessage> UploadImageAsync(int id, IFormFile formFile)
    {
        using var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(formFile.OpenReadStream());
        content.Add(streamContent, "image", formFile.FileName);

        var request = new HttpRequestMessage(HttpMethod.Post, $"{id}")
        {
            Content = content
        };

        return await httpClient.SendAsync(request);
    }
}
