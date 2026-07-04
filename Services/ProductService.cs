using NorthwindStore.Repositories;
using NorthwindStore.Models.ViewModels.Products;

namespace NorthwindStore.Services;

public class ProductService(IProductRepository repository, ICacheService cache) : IProductService
{
    public async Task<ProductListViewModel> GetAvailableProductsAsync(string? search, string? sort, int page = 1)
    {
        const int pageSize = 5;

        if (string.IsNullOrWhiteSpace(search) && string.IsNullOrWhiteSpace(sort))
        {
            var all = await cache.GetOrCreateAsync(CacheService.AvailableProducts, () => repository.GetAvailableAsync(null, null));
            var items = all.Select(ToListItem).ToList();
            var totalCount = items.Count;
            var paged = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new ProductListViewModel
            {
                Search = search,
                Sort = sort,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Products = paged
            };
        }

        var result = await repository.GetAvailablePagedAsync(search, sort, page, pageSize);
        return new ProductListViewModel
        {
            Search = search,
            Sort = sort,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount,
            Products = result.Items.Select(ToListItem).ToList()
        };
    }

    public async Task<ProductListItemViewModel?> GetProductAsync(int id)
    {
        var product = await repository.GetByIdAsync(id);
        return product is null ? null : ToListItem(product);
    }

    internal static ProductListItemViewModel ToListItem(Models.Northwind.Product product)
    {
        return new ProductListItemViewModel
        {
            ProductId = product.ProductId,
            Name = product.ProductName,
            UnitPrice = product.UnitPrice ?? 0,
            Stock = product.UnitsInStock ?? 0,
            Discontinued = product.Discontinued
        };
    }
}
