using NorthwindStore.ViewModels.Products;

namespace NorthwindStore.Interfaces;

public interface IProductService
{
    Task<ProductListViewModel> GetAvailableProductsAsync(string? search, string? sort, int page = 1);
    Task<ProductListItemViewModel?> GetProductAsync(int id);
}
