using System.ComponentModel.DataAnnotations;

namespace ECommAPI.Models;

/// <summary>
/// Represents a user in the system
/// Stores user account information and credentials
/// </summary>
public class User
{
    // Primary key - uniquely identifies each user
    public int Id { get; set; }

    // User's full name
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    // Email address - used for login
    // Must be unique (no two users can have same email)
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    // Hashed password - NEVER store plain text passwords!
    // We use BCrypt to hash passwords before storing
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    // User's phone number (optional)
    [Phone]
    public string? PhoneNumber { get; set; }

    // Delivery address (optional)
    [StringLength(500)]
    public string? Address { get; set; }

    // User role - determines permissions
    // "Customer" = regular user, "Admin" = can manage products
    [Required]
    public string Role { get; set; } = "Customer";

    // When the user account was created
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Is the account active? (can be used to disable accounts)
    public bool IsActive { get; set; } = true;

    // Navigation property - all orders placed by this user
    // We'll use this later when we build the orders system
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}