using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NorthwindStore.Models.Common;

namespace NorthwindStore.Models.Northwind;

public class Customer : SoftDeleteEntity
{
    [Key]
    [Column("customer_id")]
    [StringLength(5)]
    public string CustomerId { get; set; } = string.Empty;

    [Column("company_name")]
    [StringLength(40)]
    public string CompanyName { get; set; } = string.Empty;

    [Column("contact_name")]
    [StringLength(30)]
    public string? ContactName { get; set; }

    [Column("contact_title")]
    [StringLength(30)]
    public string? ContactTitle { get; set; }

    [Column("address")]
    [StringLength(60)]
    public string? Address { get; set; }

    [Column("city")]
    [StringLength(15)]
    public string? City { get; set; }

    [Column("country")]
    [StringLength(15)]
    public string? Country { get; set; }

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
