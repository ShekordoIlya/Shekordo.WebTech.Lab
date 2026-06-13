using Microsoft.AspNetCore.Mvc;
using Shekordo.Domain.Models;
using Shekordo.UI.Extensions;

namespace Shekordo.UI.ViewComponents;

public class CartViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
        return View(cart);
    }
}
