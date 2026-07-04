using NorthwindStore.Models.ViewModels.Orders;

namespace NorthwindStore.Services;

public interface IOrderService
{
    Task<(bool Success, string Message, int? OrderId)> ConfirmCartAsync(string userId);
    Task<OrderListViewModel> GetCustomerOrdersAsync(string customerId, int page = 1);
    Task<OrderListViewModel> GetAllOrdersAsync(int page = 1);
    Task<OrderDetailViewModel?> GetOrderDetailsAsync(int orderId, string? customerId, bool allowAnyCustomer);
}
