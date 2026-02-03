namespace ECommAPI.Models;

/// <summary>
/// Represents a product in the e-commerce system
/// </summary>
public class Product
{
    // Primary key - uniquely identifies each product
    public int Id { get; set; }

    // Product name (e.g., "iPhone 14", "Nike Air Max")
    public string Name { get; set; } = string.Empty;

    // Detailed description of the product
    public string Description { get; set; } = string.Empty;

    // Product price in USD (or your preferred currency)
    public decimal Price { get; set; }

    // URL or path to the product image
    public string ImageUrl { get; set; } = string.Empty;

    // Number of items available in stock
    public int Stock { get; set; }

    // Foreign key - links this product to a category
    public int CategoryId { get; set; }

    // Navigation property - allows us to access the category details
    // When we fetch a product, we can also get its category info
    public Category Category { get; set; } = null!;

    // When the product was added to the system
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}