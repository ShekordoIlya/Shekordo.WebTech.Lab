using Microsoft.AspNetCore.WebUtilities;
using System.Net.Http.Json;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.Blazor.Services
{
    public class ApiProductService : IProductService<Dish>
    {
        private readonly HttpClient _http;
        private List<Dish> _dishes = new();
        private int _currentPage = 1;
        private int _totalPages = 1;

        public ApiProductService(HttpClient http)
        {
            _http = http;
        }

        public IEnumerable<Dish> Products => _dishes;
        public int CurrentPage => _currentPage;
        public int TotalPages => _totalPages;

        public event Action? ListChanged;

        public async Task GetProducts(int pageNo = 1, int pageSize = 3)
        {
            // Url сервиса API
            var uri = _http.BaseAddress!.AbsoluteUri;

            // данные для Query запроса
            var queryData = new Dictionary<string, string?>
            {
                { "pageNo", pageNo.ToString() },
                { "pageSize", pageSize.ToString() }
            };

            var query = QueryString.Create(queryData!);

            // Отправить запрос к API
            var result = await _http.GetAsync(uri + query.Value);

            // В случае успешного ответа
            if (result.IsSuccessStatusCode)
            {
                // получить данные из ответа
                var responseData = await result.Content
                    .ReadFromJsonAsync<ResponseData<ListModel<Dish>>>();

                if (responseData != null && responseData.Data != null)
                {
                    // обновить параметры
                    _currentPage = responseData.Data.CurrentPage;
                    _totalPages = responseData.Data.TotalPages;
                    _dishes = responseData.Data.Items;

                    // уведомить подписчиков об изменении списка
                    ListChanged?.Invoke();
                }
            }
            // В случае ошибки
            else
            {
                _dishes = new List<Dish>();
                _currentPage = 1;
                _totalPages = 1;
            }
        }
    }
}