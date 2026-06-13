using Microsoft.EntityFrameworkCore;
using Shekordo.Domain.Entities;

namespace Shekordo.API.Data;

public static class DbInitializer
{
    public static async Task SeedData(WebApplication app)
    {
        var uri = "https://localhost:7002/";

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.MigrateAsync();

        if (context.Categories.Any() || context.Dishes.Any())
        {
            return;
        }

        var categories = new[]
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
            new()
            {
                Name = "Суп харчо",
                Description = "Острый, грузинский суп",
                Calories = 250,
                Category = categories.First(c => c.NormalizedName == "soups"),
                Image = uri + "Images/harcho.jpg"
            },
            new()
            {
                Name = "Борщ",
                Description = "Красный борщ со сметаной",
                Calories = 300,
                Category = categories.First(c => c.NormalizedName == "soups"),
                Image = uri + "Images/borsch.jpg"
            },
            new()
            {
                Name = "Цезарь",
                Description = "Салат с курицей и соусом",
                Calories = 400,
                Category = categories.First(c => c.NormalizedName == "salads"),
                Image = uri + "Images/cesar.jpg"
            },
            new()
            {
                Name = "Греческий",
                Description = "Салат с фетой и оливками",
                Calories = 350,
                Category = categories.First(c => c.NormalizedName == "salads"),
                Image = uri + "Images/greek.jpg"
            },
            new()
            {
                Name = "Морс",
                Description = "Клюквенный морс",
                Calories = 120,
                Category = categories.First(c => c.NormalizedName == "drinks"),
                Image = uri + "Images/mors.jpg"
            },
            new()
            {
                Name = "Чай",
                Description = "Зелёный чай",
                Calories = 0,
                Category = categories.First(c => c.NormalizedName == "drinks"),
                Image = uri + "Images/tea.jpg"
            }
        };

        await context.Dishes.AddRangeAsync(dishes);
        await context.SaveChangesAsync();
    }
}
