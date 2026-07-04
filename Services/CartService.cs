using NorthwindStore.Infrastructure;
using NorthwindStore.Repositories;
using NorthwindStore.Models.ViewModels.Cart;

namespace NorthwindStore.Services;

public class CartService(IHttpContextAccessor accessor, IProductRepository products) : ICartService
{
    public CartSummaryViewModel GetCart()
    {
        return Session.GetObject<CartSummaryViewModel>(SessionKeys.Cart) ?? new CartSummaryViewModel();
    }

    public async Task<(bool Success, string Message)> AddAsync(int productId, int quantity)
    {
        var validation = ValidateQuantity(quantity);
        if (!validation.Success)
        {
            return validation;
        }

        var product = await products.GetByIdAsync(productId);
        if (product is null)
        {
            return (false, "Product does not exist.");
        }
        if (product.Discontinued)
        {
            return (false, "Discontinued products cannot be purchased.");
        }
        if ((product.UnitsInStock ?? 0) <= 0)
        {
            return (false, "Product is out of stock.");
        }

        var cart = GetCart();
        var existing = cart.Items.FirstOrDefault(item => item.ProductId == productId);
        var requested = quantity + (existing?.Quantity ?? 0);
        if (requested > (product.UnitsInStock ?? 0))
        {
            return (false, "Requested quantity is greater than current stock.");
        }

        if (existing is null)
        {
            cart.Items.Add(new CartItemViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice ?? 0,
                Stock = product.UnitsInStock ?? 0,
                Quantity = quantity
            });
        }
        else
        {
            existing.Quantity = requested;
            existing.Stock = product.UnitsInStock ?? 0;
            existing.UnitPrice = product.UnitPrice ?? 0;
        }

        Save(cart);
        return (true, "Product added to cart.");
    }

    public async Task<(bool Success, string Message)> UpdateAsync(int productId, int quantity)
    {
        var validation = ValidateQuantity(quantity);
        if (!validation.Success)
        {
            return validation;
        }

        var product = await products.GetByIdAsync(productId);
        if (product is null)
        {
            return (false, "Product does not exist.");
        }
        if (product.Discontinued || (product.UnitsInStock ?? 0) <= 0)
        {
            return (false, "Product is not available.");
        }
        if (quantity > (product.UnitsInStock ?? 0))
        {
            return (false, "Requested quantity is greater than current stock.");
        }

        var cart = GetCart();
        var item = cart.Items.FirstOrDefault(line => line.ProductId == productId);
        if (item is null)
        {
            return (false, "Product is not in the cart.");
        }

        item.Quantity = quantity;
        item.Stock = product.UnitsInStock ?? 0;
        item.UnitPrice = product.UnitPrice ?? 0;
        Save(cart);
        return (true, "Cart updated.");
    }

    public void Remove(int productId)
    {
        var cart = GetCart();
        cart.Items.RemoveAll(item => item.ProductId == productId);
        Save(cart);
    }

    public void Clear()
    {
        Session.Remove(SessionKeys.Cart);
    }

    private static (bool Success, string Message) ValidateQuantity(int quantity)
    {
        return quantity <= 0
            ? (false, "Quantity must be a positive whole number.")
            : (true, string.Empty);
    }

    private ISession Session => accessor.HttpContext?.Session
        ?? throw new InvalidOperationException("Session is not available.");

    private void Save(CartSummaryViewModel cart) => Session.SetObject(SessionKeys.Cart, cart);
}
