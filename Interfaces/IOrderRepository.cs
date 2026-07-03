using NorthwindStore.Models.Common;
using NorthwindStore.Models.Northwind;

namespace NorthwindStore.Interfaces;

public interface IOrderRepository
{
    Task<IReadOnlyList<Order>> GetAllAsync();
    Task<PagedResult<Order>> GetAllPagedAsync(int page, int pageSize);
    Task<IReadOnlyList<Order>> GetByCustomerAsync(string customerId);
    Task<PagedResult<Order>> GetByCustomerPagedAsync(string customerId, int page, int pageSize);
    Task<Order?> GetDetailsAsync(int orderId);
    Task<IReadOnlyList<(Product Product, int Quantity)>> GetMostPurchasedProductsAsync(int take);
}
