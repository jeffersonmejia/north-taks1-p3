using NorthwindStore.ViewModels.Products;

namespace NorthwindStore.ViewModels.Admin;

public class InventoryReportViewModel
{
    public string Title { get; set; } = "Inventory";
    public IReadOnlyList<ProductListItemViewModel> Products { get; set; } = [];
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
