using System.Net.Http.Json;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.Blazor.Services;

public class ApiProductService(HttpClient http) : IProductService<Dish>
{
    private List<Dish> _dishes = [];
    private int _currentPage = 1;
    private int _totalPages = 1;

    public IEnumerable<Dish> Products => _dishes;
    public int CurrentPage => _currentPage;
    public int TotalPages => _totalPages;
    public event Action? ListChanged;

    public async Task GetProducts(int pageNo = 1, int pageSize = 3)
    {
        var query = QueryString.Create(new Dictionary<string, string?>
        {
            ["pageNo"] = pageNo.ToString(),
            ["pageSize"] = pageSize.ToString()
        });

        var result = await http.GetAsync(query.Value);
        if (result.IsSuccessStatusCode)
        {
            var responseData = await result.Content.ReadFromJsonAsync<ResponseData<ListModel<Dish>>>();
            if (responseData?.Data != null)
            {
                _currentPage = responseData.Data.CurrentPage;
                _totalPages = responseData.Data.TotalPages;
                _dishes = responseData.Data.Items;
                ListChanged?.Invoke();
                return;
            }
        }

        _dishes = [];
        _currentPage = 1;
        _totalPages = 1;
    }
}
