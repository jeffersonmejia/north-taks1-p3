namespace NorthwindStore.Models.ViewModels.Orders;

public class OrderDetailViewModel
{
    public int OrderId { get; set; }
    public string? CustomerId { get; set; }
    public DateTime? OrderDate { get; set; }
    public IReadOnlyList<OrderLineViewModel> Lines { get; set; } = [];
    public int TotalProducts => Lines.Sum(line => line.Quantity);
    public decimal Total => Lines.Sum(line => line.Subtotal);
}

public class OrderLineViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal Subtotal => UnitPrice * Quantity * (1 - Discount);
}
