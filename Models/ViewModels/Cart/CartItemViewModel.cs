using System.ComponentModel.DataAnnotations;

namespace NorthwindStore.Models.ViewModels.Cart;

public class CartItemViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }

    [Required]
    [Range(1, short.MaxValue)]
    public int Quantity { get; set; }

    public decimal Subtotal => UnitPrice * Quantity;
}
