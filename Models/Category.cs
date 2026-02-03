namespace ECommAPI.Models;

/// <summary>
/// Represents a product category in the e-commerce system
/// Example: Electronics, Clothing, Books, etc.
/// </summary>
public class Category
{
    // Primary key - uniquely identifies each category
    public int Id { get; set; }

    // Category name (e.g., "Electronics", "Clothing")
    public string Name { get; set; } = string.Empty;

    // Optional description of the category
    public string Description { get; set; } = string.Empty;

    // Navigation property - one category can have many products
    // This creates a relationship between Category and Product
    public ICollection<Product> Products { get; set; } = new List<Product>();
}