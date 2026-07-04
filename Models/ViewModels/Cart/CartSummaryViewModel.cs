namespace NorthwindStore.Models.ViewModels.Cart;

public class CartSummaryViewModel
{
    public List<CartItemViewModel> Items { get; set; } = [];
    public int TotalProducts => Items.Sum(item => item.Quantity);
    public decimal Total => Items.Sum(item => item.Subtotal);
    public bool IsEmpty => Items.Count == 0;
}
