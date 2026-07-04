namespace NorthwindStore.Models.ViewModels.Orders;

public class OrderSummaryViewModel
{
    public int OrderId { get; set; }
    public string? CustomerId { get; set; }
    public DateTime? OrderDate { get; set; }
    public int TotalProducts { get; set; }
    public decimal Total { get; set; }
}
