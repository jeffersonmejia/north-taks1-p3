using NorthwindStore.Models.ViewModels.Products;

namespace NorthwindStore.Services;

public interface IProductService
{
    Task<ProductListViewModel> GetAvailableProductsAsync(string? search, string? sort, int page = 1);
    Task<ProductListItemViewModel?> GetProductAsync(int id);
}
