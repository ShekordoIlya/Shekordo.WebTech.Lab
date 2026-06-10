using Microsoft.AspNetCore.Mvc;
using Shekordo.UI.Models;

namespace Shekordo.UI.Controllers;

public class HomeController : Controller
{
    private readonly List<ListDemo> _listData;

    public HomeController()
    {
        _listData = new List<ListDemo>
        {
            new ListDemo { Id = 1, Name = "Item 1" },
            new ListDemo { Id = 2, Name = "Item 2" },
            new ListDemo { Id = 3, Name = "Item 3" }
        };
    }

    public IActionResult Index()
    {
        ViewData["text"] = "Лабораторная работа №2";
        var selectList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_listData, "Id", "Name");
        return View(selectList);
    }

    [HttpPost]
    public IActionResult Index(string userName, string gender, string[] interests, int selDemo)
    {
        ViewData["text"] = "Лабораторная работа №2";
        var selectList = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_listData, "Id", "Name");


        return View(selectList);
    }
}