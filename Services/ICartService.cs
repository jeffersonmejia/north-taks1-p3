using NorthwindStore.Models.ViewModels.Cart;

namespace NorthwindStore.Services;

public interface ICartService
{
    CartSummaryViewModel GetCart();
    Task<(bool Success, string Message)> AddAsync(int productId, int quantity);
    Task<(bool Success, string Message)> UpdateAsync(int productId, int quantity);
    void Remove(int productId);
    void Clear();
}
