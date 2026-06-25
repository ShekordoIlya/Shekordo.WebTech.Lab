using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.UI.Services
{
    public class MemoryProductService : IProductService
    {
        List<Dish> _dishes;
        List<Category> _categories;
        private readonly IConfiguration _config;

        public MemoryProductService(IConfiguration config, ICategoryService categoryService)
        {
            _config = config;
            _categories = categoryService.GetCategoryListAsync().Result.Data!;
            SetupData();
        }

        /// <summary>
        /// Инициализация списков
        /// </summary>
        private void SetupData()
        {
            _dishes = new List<Dish>
            {
                new Dish
                {
                    Id = 1,
                    Name = "Суп-харчо",
                    Description = "Очень острый, невкусный",
                    Calories = 200,
                    Image = "Images/soup1.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("soups"))!.Id
                },
                new Dish
                {
                    Id = 2,
                    Name = "Борщ",
                    Description = "Много сала, без сметаны",
                    Calories = 330,
                    Image = "Images/soup2.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("soups"))!.Id
                },
                new Dish
                {
                    Id = 3,
                    Name = "Цезарь",
                    Description = "Классический салат с курицей",
                    Calories = 250,
                    Image = "Images/salad1.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("salads"))!.Id
                },
                new Dish
                {
                    Id = 4,
                    Name = "Греческий салат",
                    Description = "Свежие овощи и фета",
                    Calories = 180,
                    Image = "Images/salad2.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("salads"))!.Id
                },
                new Dish
                {
                    Id = 5,
                    Name = "Кола",
                    Description = "Газированный напиток",
                    Calories = 140,
                    Image = "Images/drink1.jpg",
                    CategoryId = _categories.Find(c => c.NormalizedName.Equals("drinks"))!.Id
                }
            };

            // Устанавливаем навигационные свойства
            foreach (var dish in _dishes)
            {
                dish.Category = _categories.Find(c => c.Id == dish.CategoryId);
            }
        }

        public Task<ResponseData<ListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            var result = new ResponseData<ListModel<Dish>>();

            // Id категории для фильтрации
            int? categoryId = null;

            // если требуется фильтрация, то найти Id категории с заданным categoryNormalizedName
            if (categoryNormalizedName != null)
            {
                categoryId = _categories.Find(c => c.NormalizedName.Equals(categoryNormalizedName))?.Id;
            }

            // Выбрать объекты, отфильтрованные по Id категории
            var data = _dishes.Where(d => categoryId == null || d.CategoryId.Equals(categoryId)).ToList();

            // получить размер страницы из конфигурации
            int pageSize = _config.GetSection("ItemsPerPage").Get<int>();

            // получить общее количество страниц
            int totalPages = (int)Math.Ceiling(data.Count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            // получить данные страницы
            var listData = new ListModel<Dish>()
            {
                Items = data.Skip((pageNo - 1) * pageSize).Take(pageSize).ToList(),
                CurrentPage = pageNo,
                TotalPages = totalPages
            };

            // поместить данные в объект результата
            result.Data = listData;

            // Если список пустой
            if (data.Count == 0)
            {
                result.Success = false;
                result.ErrorMessage = "Нет объектов в выбранной категории";
            }

            // Вернуть результат
            return Task.FromResult(result);
        }

        public Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            var dish = _dishes.Find(d => d.Id == id);
            var result = new ResponseData<Dish>();

            if (dish == null)
            {
                result.Success = false;
                result.ErrorMessage = "Объект не найден";
            }
            else
            {
                result.Data = dish;
            }

            return Task.FromResult(result);
        }

        public Task UpdateProductAsync(int id, Dish product, IFormFile? formFile)
        {
            var dish = _dishes.Find(d => d.Id == id);
            if (dish != null)
            {
                dish.Name = product.Name;
                dish.Description = product.Description;
                dish.Calories = product.Calories;
                dish.CategoryId = product.CategoryId;
                dish.Category = _categories.Find(c => c.Id == product.CategoryId);

                if (formFile != null && formFile.Length > 0)
                {
                    dish.Image = $"Images/{formFile.FileName}";
                }
            }
            return Task.CompletedTask;
        }

        public Task DeleteProductAsync(int id)
        {
            var dish = _dishes.Find(d => d.Id == id);
            if (dish != null)
            {
                _dishes.Remove(dish);
            }
            return Task.CompletedTask;
        }

        public Task<ResponseData<Dish>> CreateProductAsync(Dish product, IFormFile? formFile)
        {
            var result = new ResponseData<Dish>();

            product.Id = _dishes.Max(d => d.Id) + 1;
            product.Category = _categories.Find(c => c.Id == product.CategoryId);

            if (formFile != null && formFile.Length > 0)
            {
                product.Image = $"Images/{formFile.FileName}";
            }

            _dishes.Add(product);
            result.Data = product;

            return Task.FromResult(result);
        }
    }
}