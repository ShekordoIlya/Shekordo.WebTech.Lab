using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 

namespace Shekordo.UI.Controllers 
{
    public class HomeController : Controller
    {
        private readonly List<ListDemo> _listData;

        public HomeController()
        {
            _listData = new List<ListDemo>
            {
                new ListDemo { Id = 1, Name = "Элемент 1" },
                new ListDemo { Id = 2, Name = "Элемент 2" },
                new ListDemo { Id = 3, Name = "Элемент 3" }
            };
        }

        public IActionResult Index()
        {
            ViewData["text"] = "Лабораторная работа №2";

            SelectList data = new SelectList(_listData, "Id", "Name");

            return View(data);
        }
    }

    // Класс элемента списка (п. 3.4 d)
    public class ListDemo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Добавили инициализацию
    }
}