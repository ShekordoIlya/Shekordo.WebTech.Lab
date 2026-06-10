using Microsoft.AspNetCore.Mvc;

namespace Shekordo.UI.ViewComponents;

public class CartViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        ViewBag.TotalPrice = 125.50m;
        ViewBag.ItemCount = 3;
        return View();
    }
}