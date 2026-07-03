using Microsoft.EntityFrameworkCore;
using NorthwindStore.Data;
using NorthwindStore.Helpers;
using NorthwindStore.Interfaces;
using NorthwindStore.ViewModels.Admin;

namespace NorthwindStore.Services;

public class InventoryService(
    NorthwindContext context,
    IProductRepository products,
    ICacheService cache,
    IConfiguration configuration) : IInventoryService
{
    private const int PageSize = 5;

    public async Task<InventoryReportViewModel> GetInventoryAsync(int page = 1)
    {
        var result = await products.GetAllPagedAsync(page, PageSize);
        return new InventoryReportViewModel
        {
            Title = "All products",
            Products = result.Items.Select(ProductService.ToListItem).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }

    public async Task<InventoryReportViewModel> GetLowStockAsync(int page = 1)
    {
        var threshold = configuration.GetValue("Inventory:LowStockThreshold", Constants.DefaultLowStockThreshold);
        var all = await cache.GetOrCreateAsync(CacheService.LowStockProducts, () => products.GetLowStockAsync(threshold));
        return PageFromCache(all, page, "Low stock");
    }

    public async Task<InventoryReportViewModel> GetOutOfStockAsync(int page = 1)
    {
        var all = await cache.GetOrCreateAsync(CacheService.OutOfStockProducts, products.GetOutOfStockAsync);
        return PageFromCache(all, page, "Out of stock");
    }

    public async Task<InventoryReportViewModel> GetDiscontinuedAsync(int page = 1)
    {
        var all = await cache.GetOrCreateAsync(CacheService.DiscontinuedProducts, products.GetDiscontinuedAsync);
        return PageFromCache(all, page, "Discontinued products");
    }

    public async Task<StockUpdateViewModel?> GetStockUpdateAsync(int productId, string operation)
    {
        var product = await products.GetByIdAsync(productId);
        if (product is null)
        {
            return null;
        }

        return new StockUpdateViewModel
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            CurrentStock = product.UnitsInStock ?? 0,
            Operation = operation == "decrease" ? "decrease" : "increase"
        };
    }

    public async Task<(bool Success, string Message, StockUpdateViewModel? Model)> UpdateStockAsync(StockUpdateViewModel model, string adminName)
    {
        if (model.Quantity <= 0)
        {
            return (false, "Quantity must be positive.", model);
        }

        var product = await context.Products.FirstOrDefaultAsync(row => row.ProductId == model.ProductId);
        if (product is null)
        {
            return (false, "Product does not exist.", null);
        }

        var current = product.UnitsInStock ?? 0;
        var newStock = model.Operation == "decrease" ? current - model.Quantity : current + model.Quantity;
        if (newStock < 0)
        {
            return (false, "Stock cannot become negative.", new StockUpdateViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CurrentStock = current,
                Operation = model.Operation,
                Quantity = model.Quantity
            });
        }

        product.UnitsInStock = (short)newStock;
        await context.SaveChangesAsync();
        cache.InvalidateProducts();

        return (true, $"Stock updated by {adminName}.", new StockUpdateViewModel
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            CurrentStock = current,
            Operation = model.Operation,
            Quantity = model.Quantity,
            NewStock = newStock
        });
    }

    private static InventoryReportViewModel PageFromCache(IReadOnlyList<Models.Northwind.Product> all, int page, string title)
    {
        var items = all.Select(ProductService.ToListItem).ToList();
        var totalCount = items.Count;
        var paged = items.Skip((page - 1) * PageSize).Take(PageSize).ToList();

        return new InventoryReportViewModel
        {
            Title = title,
            Products = paged,
            Page = page,
            PageSize = PageSize,
            TotalCount = totalCount
        };
    }
}
