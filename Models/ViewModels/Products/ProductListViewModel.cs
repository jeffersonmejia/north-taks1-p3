namespace NorthwindStore.Models.ViewModels.Products;

public class ProductListViewModel
{
    public string? Search { get; set; }
    public string? Sort { get; set; }
    public IReadOnlyList<ProductListItemViewModel> Products { get; set; } = [];
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 5;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}

public class ProductListItemViewModel
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }
    public bool Discontinued { get; set; }
    public bool IsAvailable => Stock > 0 && !Discontinued;
}
