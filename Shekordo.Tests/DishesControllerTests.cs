using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Shekordo.API.Controllers;
using Shekordo.API.Data;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;

namespace Shekordo.Tests;

public class DishesControllerTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<AppDbContext> _contextOptions;
    private readonly IWebHostEnvironment _environment;

    public DishesControllerTests()
    {
        _environment = Substitute.For<IWebHostEnvironment>();
        _environment.WebRootPath = Path.GetTempPath();

        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new AppDbContext(_contextOptions);
        context.Database.EnsureCreated();

        var categories = new[]
        {
            new Category { Name = "Супы", NormalizedName = "soups" },
            new Category { Name = "Основные", NormalizedName = "main-dishes" }
        };
        context.Categories.AddRange(categories);
        context.SaveChanges();

        var dishes = new List<Dish>
        {
            new() { Name = "A", Description = "", Calories = 0, Category = categories[0] },
            new() { Name = "B", Description = "", Calories = 0, Category = categories[0] },
            new() { Name = "C", Description = "", Calories = 0, Category = categories[1] },
            new() { Name = "D", Description = "", Calories = 0, Category = categories[1] },
            new() { Name = "E", Description = "", Calories = 0, Category = categories[1] }
        };
        context.Dishes.AddRange(dishes);
        context.SaveChanges();
    }

    public void Dispose() => _connection.Dispose();

    private AppDbContext CreateContext() => new(_contextOptions);

    [Fact]
    public async Task ControllerFiltersCategory()
    {
        using var context = CreateContext();
        var category = context.Categories.First();
        var controller = new DishesController(context, _environment);

        var response = await controller.GetDishes(category.NormalizedName);
        var responseData = response.Value!;
        var dishesList = responseData.Data!.Items;

        Assert.True(dishesList.All(d => d.CategoryId == category.Id));
    }

    [Theory]
    [InlineData(2, 3)]
    [InlineData(3, 2)]
    public async Task ControllerReturnsCorrectPagesCount(int size, int qty)
    {
        using var context = CreateContext();
        var controller = new DishesController(context, _environment);

        var response = await controller.GetDishes(null, 1, size);
        var totalPages = response.Value!.Data!.TotalPages;

        Assert.Equal(qty, totalPages);
    }

    [Fact]
    public async Task ControllerReturnsCorrectPage()
    {
        using var context = CreateContext();
        var controller = new DishesController(context, _environment);
        var firstItem = context.Dishes.ToArray()[3];

        var response = await controller.GetDishes(null, 2);
        var responseData = response.Value!;
        var dishesList = responseData.Data!.Items;

        Assert.Equal(2, responseData.Data.CurrentPage);
        Assert.Equal(2, dishesList.Count);
        Assert.Equal(firstItem.Id, dishesList[0].Id);
    }
}
