namespace ECommAPI.Models;

/// <summary>
/// Represents an item in a user's shopping cart
/// Stores which product and how many units the user wants to buy
/// </summary>
public class CartItem
{
    // Primary key
    public int Id { get; set; }

    // Foreign key - which product is in the cart
    public int ProductId { get; set; }

    // Navigation property - access full product details
    public Product Product { get; set; } = null!;

    // How many units of this product the user wants
    public int Quantity { get; set; }

    // Session ID to track different users' carts
    // In a real app, this would be a user ID after login
    public string SessionId { get; set; } = string.Empty;

    // When this item was added to the cart
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}