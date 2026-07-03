using NorthwindStore.Models.Common;
using NorthwindStore.Models.Northwind;

namespace NorthwindStore.Interfaces;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAvailableAsync(string? search, string? sort);
    Task<PagedResult<Product>> GetAvailablePagedAsync(string? search, string? sort, int page, int pageSize);
    Task<IReadOnlyList<Product>> GetAllAsync();
    Task<PagedResult<Product>> GetAllPagedAsync(int page, int pageSize);
    Task<IReadOnlyList<Product>> GetLowStockAsync(int threshold);
    Task<PagedResult<Product>> GetLowStockPagedAsync(int threshold, int page, int pageSize);
    Task<IReadOnlyList<Product>> GetOutOfStockAsync();
    Task<PagedResult<Product>> GetOutOfStockPagedAsync(int page, int pageSize);
    Task<IReadOnlyList<Product>> GetDiscontinuedAsync();
    Task<PagedResult<Product>> GetDiscontinuedPagedAsync(int page, int pageSize);
    Task<Product?> GetByIdAsync(int id);
    Task<bool> HasSufficientStockAsync(int productId, int quantity);
}
