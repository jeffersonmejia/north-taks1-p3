using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NorthwindStore.Data;
using NorthwindStore.Repositories;
using NorthwindStore.Models.Identity;
using NorthwindStore.Models.Northwind;
using NorthwindStore.Models.ViewModels.Orders;

namespace NorthwindStore.Services;

public class OrderService(
    NorthwindContext context,
    UserManager<ApplicationUser> users,
    ICartService cart,
    IOrderRepository orders,
    ICacheService cache) : IOrderService
{
    public async Task<(bool Success, string Message, int? OrderId)> ConfirmCartAsync(string userId)
    {
        var user = await users.FindByIdAsync(userId);
        if (user?.CustomerId is null)
        {
            return (false, "Customer profile is not available.", null);
        }

        var currentCart = cart.GetCart();
        if (currentCart.IsEmpty)
        {
            return (false, "The cart has no products.", null);
        }

        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var order = new Order
            {
                CustomerId = user.CustomerId,
                OrderDate = DateTime.UtcNow,
                RequiredDate = DateTime.UtcNow.AddDays(7),
                ShipName = user.FullName ?? user.Email,
                ShipCity = "Online"
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            foreach (var item in currentCart.Items)
            {
                var product = await context.Products.FirstOrDefaultAsync(row => row.ProductId == item.ProductId);
                if (product is null)
                {
                    throw new InvalidOperationException("A product in the cart no longer exists.");
                }
                if (product.Discontinued)
                {
                    throw new InvalidOperationException($"{product.ProductName} is discontinued.");
                }
                if ((product.UnitsInStock ?? 0) < item.Quantity)
                {
                    throw new InvalidOperationException($"Stock changed for {product.ProductName}.");
                }

                context.OrderDetails.Add(new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    UnitPrice = product.UnitPrice ?? 0,
                    Quantity = (short)item.Quantity,
                    Discount = 0
                });

                product.UnitsInStock = (short)((product.UnitsInStock ?? 0) - item.Quantity);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            cart.Clear();
            cache.InvalidateProducts();
            return (true, "Purchase confirmed.", order.OrderId);
        }
        catch
        {
            await transaction.RollbackAsync();
            return (false, "The purchase could not be completed. Please review the cart and try again.", null);
        }
    }

    public async Task<OrderListViewModel> GetCustomerOrdersAsync(string customerId, int page = 1)
    {
        var result = await orders.GetByCustomerPagedAsync(customerId, page, 5);
        return new OrderListViewModel
        {
            Orders = result.Items.Select(ToSummary).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<OrderListViewModel> GetAllOrdersAsync(int page = 1)
    {
        var result = await orders.GetAllPagedAsync(page, 5);
        return new OrderListViewModel
        {
            Orders = result.Items.Select(ToSummary).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<OrderDetailViewModel?> GetOrderDetailsAsync(int orderId, string? customerId, bool allowAnyCustomer)
    {
        var order = await orders.GetDetailsAsync(orderId);
        if (order is null || (!allowAnyCustomer && order.CustomerId != customerId))
        {
            return null;
        }

        return new OrderDetailViewModel
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            Lines = order.OrderDetails.Select(detail => new OrderLineViewModel
            {
                ProductId = detail.ProductId,
                ProductName = detail.Product?.ProductName ?? $"Product {detail.ProductId}",
                UnitPrice = detail.UnitPrice,
                Quantity = detail.Quantity,
                Discount = (decimal)detail.Discount
            }).ToList()
        };
    }

    private static OrderSummaryViewModel ToSummary(Order order)
    {
        return new OrderSummaryViewModel
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            OrderDate = order.OrderDate,
            TotalProducts = order.OrderDetails.Sum(detail => detail.Quantity),
            Total = order.OrderDetails.Sum(detail => detail.UnitPrice * detail.Quantity * (decimal)(1 - detail.Discount))
        };
    }
}
