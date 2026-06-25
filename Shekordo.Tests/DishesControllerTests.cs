using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using NSubstitute;
using Shekordo.API.Controllers;
using Shekordo.API.Data;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;
using Xunit;

namespace Shekordo.Tests
{
    public class DishesControllerTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<AppDbContext> _contextOptions;
        private readonly IWebHostEnvironment _environment;

        public DishesControllerTests()
        {
            _environment = Substitute.For<IWebHostEnvironment>();

            // Create and open a connection. This creates the SQLite in-memory database, 
            // which will persist until the connection is closed at the end of the test.
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            // These options will be used by the context instances in this test suite
            _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Create the schema and seed some data
            using var context = new AppDbContext(_contextOptions);
            context.Database.EnsureCreated();

            var categories = new Category[]
            {
                new Category { Name = "", NormalizedName = "soups" },
                new Category { Name = "", NormalizedName = "main-dishes" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            var dishes = new List<Dish>
            {
                new Dish { Name = "", Description = "", Calories = 0,
                    Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("soups")) },
                new Dish { Name = "", Description = "", Calories = 0,
                    Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("soups")) },
                new Dish { Name = "", Description = "", Calories = 0,
                    Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("main-dishes")) },
                new Dish { Name = "", Description = "", Calories = 0,
                    Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("main-dishes")) },
                new Dish { Name = "", Description = "", Calories = 0,
                    Category = categories.FirstOrDefault(c => c.NormalizedName.Equals("main-dishes")) }
            };
            context.AddRange(dishes);
            context.SaveChanges();
        }

        public void Dispose() => _connection?.Dispose();

        AppDbContext CreateContext() => new AppDbContext(_contextOptions);

        // Проверка фильтра по категории
        [Fact]
        public async void ControllerFiltersCategory()
        {
            // arrange
            using var context = CreateContext();
            var category = context.Categories.First();
            var controller = new DishesController(context, _environment);

            // act
            var response = await controller.GetDishes(category.NormalizedName);
            var responseData = response.Value as ResponseData<ListModel<Dish>>;
            var dishesList = responseData!.Data.Items;

            // assert
            Assert.True(dishesList.All(d => d.CategoryId == category.Id));
        }

        // Проверка подсчета количества страниц
        [Theory]
        [InlineData(2, 3)]
        [InlineData(3, 2)]
        public async void ControllerReturnsCorrectPagesCount(int size, int qty)
        {
            using var context = CreateContext();
            var controller = new DishesController(context, _environment);

            // act
            var response = await controller.GetDishes(null, 1, size);
            var responseData = response.Value as ResponseData<ListModel<Dish>>;
            var totalPages = responseData!.Data.TotalPages;

            // assert
            Assert.Equal(qty, totalPages);
        }

        [Fact]
        public async void ControllerReturnsCorrectPage()
        {
            using var context = CreateContext();
            var controller = new DishesController(context, _environment);

            // При размере страницы 3 и общем количестве объектов 5
            // на 2-й странице должно быть 2 объекта
            int itemsInPage = 2;
            Dish firstItem = context.Dishes.ToArray()[3];

            // act
            var response = await controller.GetDishes(null, 2);
            var responseData = response.Value as ResponseData<ListModel<Dish>>;
            var dishesList = responseData!.Data.Items;
            var currentPage = responseData.Data.CurrentPage;

            // assert
            Assert.Equal(2, currentPage);
            Assert.Equal(2, dishesList.Count);
            Assert.Equal(firstItem.Id, dishesList[0].Id);
        }
    }
}