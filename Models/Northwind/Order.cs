using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NorthwindStore.Models.Common;

namespace NorthwindStore.Models.Northwind;

public class Order : SoftDeleteEntity
{
    [Key]
    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("customer_id")]
    [StringLength(5)]
    public string? CustomerId { get; set; }

    [Column("employee_id")]
    public int? EmployeeId { get; set; }

    [Column("order_date")]
    public DateTime? OrderDate { get; set; }

    [Column("required_date")]
    public DateTime? RequiredDate { get; set; }

    [Column("shipped_date")]
    public DateTime? ShippedDate { get; set; }

    [Column("ship_name")]
    [StringLength(40)]
    public string? ShipName { get; set; }

    [Column("ship_address")]
    [StringLength(60)]
    public string? ShipAddress { get; set; }

    [Column("ship_city")]
    [StringLength(15)]
    public string? ShipCity { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
