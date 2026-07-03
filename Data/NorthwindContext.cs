using Microsoft.EntityFrameworkCore;
using NorthwindStore.Models.Northwind;

namespace NorthwindStore.Data;

public class NorthwindContext(DbContextOptions<NorthwindContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasQueryFilter(product => !product.IsDeleted);
            entity.Property(product => product.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            entity.Property(product => product.DeletedAt).HasColumnName("deleted_at");
            entity.Property(product => product.DeletedBy).HasColumnName("deleted_by");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("customers");
            entity.HasQueryFilter(customer => !customer.IsDeleted);
            entity.Property(customer => customer.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            entity.Property(customer => customer.DeletedAt).HasColumnName("deleted_at");
            entity.Property(customer => customer.DeletedBy).HasColumnName("deleted_by");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasQueryFilter(order => !order.IsDeleted);
            entity.Property(order => order.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            entity.Property(order => order.DeletedAt).HasColumnName("deleted_at");
            entity.Property(order => order.DeletedBy).HasColumnName("deleted_by");
            entity.HasOne(order => order.Customer)
                .WithMany(customer => customer.Orders)
                .HasForeignKey(order => order.CustomerId);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("order_details");
            entity.HasKey(detail => new { detail.OrderId, detail.ProductId });
            entity.HasOne(detail => detail.Order)
                .WithMany(order => order.OrderDetails)
                .HasForeignKey(detail => detail.OrderId);
            entity.HasOne(detail => detail.Product)
                .WithMany(product => product.OrderDetails)
                .HasForeignKey(detail => detail.ProductId);
        });
    }
}
