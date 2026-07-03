using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NorthwindStore.Models.Common;

namespace NorthwindStore.Models.Northwind;

public class Product : SoftDeleteEntity
{
    [Key]
    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("product_name")]
    [StringLength(40)]
    public string ProductName { get; set; } = string.Empty;

    [Column("supplier_id")]
    public int? SupplierId { get; set; }

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("quantity_per_unit")]
    [StringLength(20)]
    public string? QuantityPerUnit { get; set; }

    [Column("unit_price")]
    public decimal? UnitPrice { get; set; }

    [Column("units_in_stock")]
    public short? UnitsInStock { get; set; }

    [Column("units_on_order")]
    public short? UnitsOnOrder { get; set; }

    [Column("reorder_level")]
    public short? ReorderLevel { get; set; }

    [Column("discontinued")]
    public bool Discontinued { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
