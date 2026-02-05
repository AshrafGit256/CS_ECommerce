using Microsoft.EntityFrameworkCore;
using ECommAPI.Models;

namespace ECommAPI.Data;

/// <summary>
/// Database context - manages the connection to the database
/// and provides access to our database tables (DbSets)
/// </summary>
public class AppDbContext : DbContext
{
    // Constructor - receives database configuration options
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSet represents a table in the database
    // Each DbSet<T> becomes a table, and each T instance becomes a row

    // Categories table - stores all product categories
    public DbSet<Category> Categories { get; set; }

    // Products table - stores all products
    public DbSet<Product> Products { get; set; }

    // CartItems table - stores shopping cart items
    public DbSet<CartItem> CartItems { get; set; }

    // Users table - stores user accounts
    public DbSet<User> Users { get; set; }

    // Orders table - stores customer orders
    public DbSet<Order> Orders { get; set; }

    // OrderItems table - stores items in each order
    public DbSet<OrderItem> OrderItems { get; set; }

    /// <summary>
    /// This method configures the database model and relationships
    /// It's called when creating/updating the database
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            // Set precision for decimal Price field (2 decimal places)
            entity.Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            // Configure the relationship between Product and Category
            // One Category has many Products
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); // If category is deleted, delete its products too
        });

        // Seed initial data - this adds sample data when database is created
        // This is helpful for testing and development

        // Add sample categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and gadgets" },
            new Category { Id = 2, Name = "Clothing", Description = "Fashion and apparel" },
            new Category { Id = 3, Name = "Books", Description = "Books and literature" },
            new Category { Id = 4, Name = "Home & Garden", Description = "Home improvement and gardening" }
        );

        // Add sample products
        modelBuilder.Entity<Product>().HasData(
            // Electronics
            new Product
            {
                Id = 1,
                Name = "Laptop HP Pavilion",
                Description = "15.6 inch, Intel i5, 8GB RAM, 256GB SSD",
                Price = 699.99m,
                ImageUrl = "https://via.placeholder.com/300x300?text=Laptop",
                Stock = 15,
                CategoryId = 1,
                CreatedAt = new DateTime(2024, 1, 1)  // Fixed date instead of DateTime.UtcNow
            },
            new Product
            {
                Id = 2,
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse with USB receiver",
                Price = 25.99m,
                ImageUrl = "https://via.placeholder.com/300x300?text=Mouse",
                Stock = 50,
                CategoryId = 1,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = 3,
                Name = "Smartphone Samsung Galaxy",
                Description = "6.5 inch display, 128GB storage, 5G enabled",
                Price = 599.99m,
                ImageUrl = "https://via.placeholder.com/300x300?text=Phone",
                Stock = 30,
                CategoryId = 1,
                CreatedAt = new DateTime(2024, 1, 1)
            },

            // Clothing
            new Product
            {
                Id = 4,
                Name = "Men's T-Shirt",
                Description = "100% cotton, available in multiple colors",
                Price = 19.99m,
                ImageUrl = "https://via.placeholder.com/300x300?text=TShirt",
                Stock = 100,
                CategoryId = 2,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = 5,
                Name = "Women's Jeans",
                Description = "Slim fit denim jeans",
                Price = 49.99m,
                ImageUrl = "https://via.placeholder.com/300x300?text=Jeans",
                Stock = 60,
                CategoryId = 2,
                CreatedAt = new DateTime(2024, 1, 1)
            },

            // Books
            new Product
            {
                Id = 6,
                Name = "Clean Code",
                Description = "A Handbook of Agile Software Craftsmanship",
                Price = 32.99m,
                ImageUrl = "https://via.placeholder.com/300x300?text=Book",
                Stock = 25,
                CategoryId = 3,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = 7,
                Name = "The Pragmatic Programmer",
                Description = "Your Journey to Mastery",
                Price = 39.99m,
                ImageUrl = "https://via.placeholder.com/300x300?text=Book",
                Stock = 20,
                CategoryId = 3,
                CreatedAt = new DateTime(2024, 1, 1)
            }
        );

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            // Email must be unique - no two users can have the same email
            entity.HasIndex(u => u.Email).IsUnique();

            // Configure the relationship: One User has many Orders
            entity.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If user is deleted, delete their orders too
        });

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            // Set precision for TotalAmount (2 decimal places)
            entity.Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            // Configure the relationship: One Order has many OrderItems
            entity.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure OrderItem entity
        modelBuilder.Entity<OrderItem>(entity =>
        {
            // Set precision for PriceAtPurchase
            entity.Property(oi => oi.PriceAtPurchase)
                .HasColumnType("decimal(18,2)");

            // Configure relationship with Product
            entity.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Don't delete product if it's in an order
        });
    }
}