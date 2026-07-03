using System.ComponentModel.DataAnnotations;

namespace NorthwindStore.ViewModels.Admin;

public class StockUpdateViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public string Operation { get; set; } = "increase";

    [Required]
    [Range(1, short.MaxValue, ErrorMessage = "Quantity must be a positive whole number.")]
    public int Quantity { get; set; }

    public int? NewStock { get; set; }
}
