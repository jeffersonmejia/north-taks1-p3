using Microsoft.EntityFrameworkCore;
using NorthwindStore.Data;

using NorthwindStore.Models.Common;
using NorthwindStore.Models.Northwind;

namespace NorthwindStore.Repositories;

public class OrderRepository(NorthwindContext context) : IOrderRepository
{
    public async Task<IReadOnlyList<Order>> GetAllAsync()
    {
        return await IncludeOrderGraph()
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync();
    }

    public async Task<PagedResult<Order>> GetAllPagedAsync(int page, int pageSize)
    {
        return await ToPagedAsync(IncludeOrderGraph().OrderByDescending(order => order.OrderDate), page, pageSize);
    }

    public async Task<IReadOnlyList<Order>> GetByCustomerAsync(string customerId)
    {
        return await IncludeOrderGraph()
            .Where(order => order.CustomerId == customerId)
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync();
    }

    public async Task<PagedResult<Order>> GetByCustomerPagedAsync(string customerId, int page, int pageSize)
    {
        return await ToPagedAsync(
            IncludeOrderGraph().Where(order => order.CustomerId == customerId)
                .OrderByDescending(order => order.OrderDate),
            page, pageSize);
    }

    public Task<Order?> GetDetailsAsync(int orderId)
    {
        return IncludeOrderGraph()
            .FirstOrDefaultAsync(order => order.OrderId == orderId);
    }

    public async Task<IReadOnlyList<(Product Product, int Quantity)>> GetMostPurchasedProductsAsync(int take)
    {
        var rows = await context.OrderDetails
            .AsNoTracking()
            .Where(detail => detail.Product != null)
            .GroupBy(detail => detail.Product!)
            .Select(group => new { Product = group.Key, Quantity = group.Sum(item => item.Quantity) })
            .OrderByDescending(row => row.Quantity)
            .Take(take)
            .ToListAsync();

        return rows.Select(row => (row.Product, (int)row.Quantity)).ToList();
    }

    private IQueryable<Order> IncludeOrderGraph()
    {
        return context.Orders
            .AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.OrderDetails)
            .ThenInclude(detail => detail.Product);
    }

    private static async Task<PagedResult<T>> ToPagedAsync<T>(IQueryable<T> query, int page, int pageSize)
    {
        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
