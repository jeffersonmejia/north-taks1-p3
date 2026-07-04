using NorthwindStore.Models.ViewModels.Admin;

namespace NorthwindStore.Services;

public interface IInventoryService
{
    Task<InventoryReportViewModel> GetInventoryAsync(int page = 1);
    Task<InventoryReportViewModel> GetLowStockAsync(int page = 1);
    Task<InventoryReportViewModel> GetOutOfStockAsync(int page = 1);
    Task<InventoryReportViewModel> GetDiscontinuedAsync(int page = 1);
    Task<StockUpdateViewModel?> GetStockUpdateAsync(int productId, string operation);
    Task<(bool Success, string Message, StockUpdateViewModel? Model)> UpdateStockAsync(StockUpdateViewModel model, string adminName);
}
