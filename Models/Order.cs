namespace ECommAPI.Models;

/// <summary>
/// Represents a customer order
/// This will be fully implemented when we build the orders system
/// For now, we just need the basic structure for the relationship with User
/// </summary>
public class Order
{
    // Primary key
    public int Id { get; set; }

    // Foreign key - which user placed this order
    public int UserId { get; set; }

    // Navigation property - access user details
    public User User { get; set; } = null!;

    // Order date
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    // Total amount paid
    public decimal TotalAmount { get; set; }

    // Order status (Pending, Processing, Shipped, Delivered, Cancelled)
    public string Status { get; set; } = "Pending";

    // Delivery address for this order
    public string DeliveryAddress { get; set; } = string.Empty;

    // Order items (products in this order)
    // We'll implement this fully later
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

/// <summary>
/// Represents a single item in an order
/// Links an order to a product with quantity and price
/// </summary>
public class OrderItem
{
    public int Id { get; set; }

    // Foreign key - which order this item belongs to
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    // Foreign key - which product was ordered
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    // Quantity ordered
    public int Quantity { get; set; }

    // Price at the time of purchase (stored because product price might change later)
    public decimal PriceAtPurchase { get; set; }
}