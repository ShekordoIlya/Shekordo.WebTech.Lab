using Microsoft.AspNetCore.Mvc;
using Shekordo.Domain.Models;
using Shekordo.UI.Extensions;
using Shekordo.UI.Services;

namespace Shekordo.UI.Controllers;

public class CartController(IProductService productService) : Controller
{
    [Route("[controller]/add/{id:int}")]
    public async Task<IActionResult> Add(int id, string returnUrl)
    {
        var data = await productService.GetProductByIdAsync(id);
        if (data.Success && data.Data != null)
        {
            var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
            cart.AddToCart(data.Data);
            HttpContext.Session.Set("cart", cart);
        }

        return Redirect(returnUrl);
    }

    [Route("[controller]/remove/{id:int}")]
    public IActionResult Remove(int id)
    {
        var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
        cart.RemoveItems(id);
        HttpContext.Session.Set("cart", cart);
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Index()
    {
        var cart = HttpContext.Session.Get<Cart>("cart") ?? new Cart();
        return View(cart.CartItems);
    }
}
