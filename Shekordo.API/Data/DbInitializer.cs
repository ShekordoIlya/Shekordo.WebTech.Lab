using Microsoft.EntityFrameworkCore;
using Shekordo.Domain.Entities;

namespace Shekordo.API.Data
{
    public static class DbInitializer
    {
        public static async Task SeedData(WebApplication app)
        {
            // Uri проекта API
            var uri = "https://localhost:7002/";

            // Получение контекста БД
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Выполнение миграций
            await context.Database.MigrateAsync();

            // Заполнение данными (только если таблицы пустые)
            if (!context.Categories.Any() && !context.Dishes.Any())
            {
                var categories = new Category[]
                {
                    new Category { Name = "Стартеры", NormalizedName = "starters" },
                    new Category { Name = "Салаты", NormalizedName = "salads" },
                    new Category { Name = "Супы", NormalizedName = "soups" },
                    new Category { Name = "Основные блюда", NormalizedName = "main-dishes" },
                    new Category { Name = "Напитки", NormalizedName = "drinks" },
                    new Category { Name = "Десерты", NormalizedName = "desserts" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                var dishes = new List<Dish>
                {
                    new Dish
                    {
                        Name = "Суп-харчо",
                        Description = "Очень острый, невкусный",
                        Calories = 200,
                        Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("soups")),
                        Image = uri + "Images/soup1.jpg"
                    },
                    new Dish
                    {
                        Name = "Борщ",
                        Description = "Много сала, без сметаны",
                        Calories = 330,
                        Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("soups")),
                        Image = uri + "Images/soup2.jpg"
                    },
                    new Dish
                    {
                        Name = "Цезарь",
                        Description = "Классический салат с курицей",
                        Calories = 250,
                        Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("salads")),
                        Image = uri + "Images/salad1.jpg"
                    },
                    new Dish
                    {
                        Name = "Греческий салат",
                        Description = "Свежие овощи и фета",
                        Calories = 180,
                        Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("salads")),
                        Image = uri + "Images/salad2.jpg"
                    },
                    new Dish
                    {
                        Name = "Кола",
                        Description = "Газированный напиток",
                        Calories = 140,
                        Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("drinks")),
                        Image = uri + "Images/drink1.jpg"
                    }
                };

                await context.Dishes.AddRangeAsync(dishes);
                await context.SaveChangesAsync();
            }
        }
    }
}