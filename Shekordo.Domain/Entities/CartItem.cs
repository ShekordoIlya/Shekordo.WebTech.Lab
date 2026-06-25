namespace Shekordo.Domain.Entities
{
    public class CartItem
    {
        public Dish Item { get; set; } = default!;
        public int Qty { get; set; }
    }
}