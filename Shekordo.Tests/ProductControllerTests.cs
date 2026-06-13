using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Shekordo.Domain.Entities;
using Shekordo.Domain.Models;
using Shekordo.UI.Controllers;
using Shekordo.UI.Services;

namespace Shekordo.Tests;

public class ProductControllerTests
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    public ProductControllerTests()
    {
        _categoryService = Substitute.For<ICategoryService>();
        var categoriesResponse = new ResponseData<List<Category>>
        {
            Data =
            [
                new Category { Id = 1, Name = "Стартеры", NormalizedName = "starters" },
                new Category { Id = 2, Name = "Салаты", NormalizedName = "salads" },
                new Category { Id = 3, Name = "Супы", NormalizedName = "soups" },
                new Category { Id = 4, Name = "Основные блюда", NormalizedName = "main-dishes" },
                new Category { Id = 5, Name = "Напитки", NormalizedName = "drinks" },
                new Category { Id = 6, Name = "Десерты", NormalizedName = "desserts" }
            ]
        };
        _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoriesResponse));

        _productService = Substitute.For<IProductService>();
        var dishes = new List<Dish>
        {
            new() { Id = 1 },
            new() { Id = 2 },
            new() { Id = 3 },
            new() { Id = 4 },
            new() { Id = 5 }
        };
        _productService.GetProductListAsync(Arg.Any<string?>(), Arg.Any<int>())
            .Returns(new ResponseData<ListModel<Dish>>
            {
                Data = new ListModel<Dish> { Items = dishes }
            });
    }

    [Fact]
    public async Task IndexPutsCategoriesToViewData()
    {
        var controller = new ProductController(_categoryService, _productService);
        var response = await controller.Index(null);

        var view = Assert.IsType<ViewResult>(response);
        var categories = Assert.IsType<List<Category>>(view.ViewData["categories"]);
        Assert.Equal(6, categories.Count);
        Assert.Equal("Все", view.ViewData["currentCategory"]);
    }

    [Fact]
    public async Task IndexSetsCorrectCurrentCategory()
    {
        var categories = await _categoryService.GetCategoryListAsync();
        var currentCategory = categories.Data![0];
        var controller = new ProductController(_categoryService, _productService);

        var response = await controller.Index(currentCategory.NormalizedName);

        var view = Assert.IsType<ViewResult>(response);
        Assert.Equal(currentCategory.Name, view.ViewData["currentCategory"]);
    }

    [Fact]
    public async Task IndexReturnsNotFound()
    {
        const string errorMessage = "Test error";
        var categoriesResponse = new ResponseData<List<Category>>
        {
            Success = false,
            ErrorMessage = errorMessage
        };
        _categoryService.GetCategoryListAsync().Returns(Task.FromResult(categoriesResponse));

        var controller = new ProductController(_categoryService, _productService);
        var response = await controller.Index(null);

        var result = Assert.IsType<NotFoundObjectResult>(response);
        Assert.Equal(errorMessage, result.Value?.ToString());
    }
}
