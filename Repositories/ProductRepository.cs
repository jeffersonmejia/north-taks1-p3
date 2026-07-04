using Microsoft.EntityFrameworkCore;
using NorthwindStore.Data;

using NorthwindStore.Models.Common;
using NorthwindStore.Models.Northwind;

namespace NorthwindStore.Repositories;

public class ProductRepository(NorthwindContext context) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAvailableAsync(string? search, string? sort)
    {
        return await BuildAvailableQuery(search, sort).ToListAsync();
    }

    public async Task<PagedResult<Product>> GetAvailablePagedAsync(string? search, string? sort, int page, int pageSize)
    {
        return await ToPagedAsync(BuildAvailableQuery(search, sort), page, pageSize);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync()
    {
        return await context.Products
            .AsNoTracking()
            .OrderBy(product => product.ProductName)
            .ToListAsync();
    }

    public async Task<PagedResult<Product>> GetAllPagedAsync(int page, int pageSize)
    {
        return await ToPagedAsync(
            context.Products.AsNoTracking().OrderBy(product => product.ProductName),
            page, pageSize);
    }

    public async Task<IReadOnlyList<Product>> GetLowStockAsync(int threshold)
    {
        return await BuildLowStockQuery(threshold).ToListAsync();
    }

    public async Task<PagedResult<Product>> GetLowStockPagedAsync(int threshold, int page, int pageSize)
    {
        return await ToPagedAsync(BuildLowStockQuery(threshold), page, pageSize);
    }

    public async Task<IReadOnlyList<Product>> GetOutOfStockAsync()
    {
        return await BuildOutOfStockQuery().ToListAsync();
    }

    public async Task<PagedResult<Product>> GetOutOfStockPagedAsync(int page, int pageSize)
    {
        return await ToPagedAsync(BuildOutOfStockQuery(), page, pageSize);
    }

    public async Task<IReadOnlyList<Product>> GetDiscontinuedAsync()
    {
        return await BuildDiscontinuedQuery().ToListAsync();
    }

    public async Task<PagedResult<Product>> GetDiscontinuedPagedAsync(int page, int pageSize)
    {
        return await ToPagedAsync(BuildDiscontinuedQuery(), page, pageSize);
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        return context.Products.FirstOrDefaultAsync(product => product.ProductId == id);
    }

    public Task<bool> HasSufficientStockAsync(int productId, int quantity)
    {
        return context.Products.AnyAsync(product =>
            product.ProductId == productId &&
            !product.Discontinued &&
            (product.UnitsInStock ?? 0) >= quantity);
    }

    private IQueryable<Product> BuildAvailableQuery(string? search, string? sort)
    {
        var query = context.Products
            .AsNoTracking()
            .Where(product => !product.Discontinued && (product.UnitsInStock ?? 0) > 0);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(product => EF.Functions.ILike(product.ProductName, $"%{search}%"));
        }

        query = sort switch
        {
            "price" => query.OrderBy(product => product.UnitPrice),
            "price_desc" => query.OrderByDescending(product => product.UnitPrice),
            "name_desc" => query.OrderByDescending(product => product.ProductName),
            _ => query.OrderBy(product => product.ProductName)
        };

        return query;
    }

    private IQueryable<Product> BuildLowStockQuery(int threshold)
    {
        return context.Products
            .AsNoTracking()
            .Where(product => !product.Discontinued && (product.UnitsInStock ?? 0) > 0 && (product.UnitsInStock ?? 0) <= threshold)
            .OrderBy(product => product.UnitsInStock)
            .ThenBy(product => product.ProductName);
    }

    private IQueryable<Product> BuildOutOfStockQuery()
    {
        return context.Products
            .AsNoTracking()
            .Where(product => !product.Discontinued && (product.UnitsInStock ?? 0) <= 0)
            .OrderBy(product => product.ProductName);
    }

    private IQueryable<Product> BuildDiscontinuedQuery()
    {
        return context.Products
            .AsNoTracking()
            .Where(product => product.Discontinued)
            .OrderBy(product => product.ProductName);
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
