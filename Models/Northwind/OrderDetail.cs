using System.ComponentModel.DataAnnotations.Schema;

namespace NorthwindStore.Models.Northwind;

public class OrderDetail
{
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("unit_price")]
    public decimal UnitPrice { get; set; }

    [Column("quantity")]
    public short Quantity { get; set; }

    [Column("discount")]
    public float Discount { get; set; }

    public Order? Order { get; set; }
    public Product? Product { get; set; }
}
