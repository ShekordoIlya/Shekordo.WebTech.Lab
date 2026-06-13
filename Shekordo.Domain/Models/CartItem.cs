using Shekordo.Domain.Entities;

namespace Shekordo.Domain.Models;

public class CartItem
{
    public Dish Item { get; set; } = default!;
    public int Qty { get; set; }
}
