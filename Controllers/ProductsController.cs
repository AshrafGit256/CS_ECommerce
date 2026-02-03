using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommAPI.Data;
using ECommAPI.Models;

namespace ECommAPI.Controllers;

/// <summary>
/// API Controller for managing products
/// Base route: /api/products
/// This is the main controller for the e-commerce system
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // Database context - injected automatically
    private readonly AppDbContext _context;

    // Constructor
    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET: api/products
    /// Returns all products with their category information
    /// Frontend will call this to display the product catalog
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        // Fetch all products from database
        // Include() loads the related Category for each product
        // This is called "eager loading" - we load related data in one query
        var products = await _context.Products
            .Include(p => p.Category)  // Load category details for each product
            .OrderBy(p => p.Name)      // Sort products by name
            .ToListAsync();

        // Return 200 OK with the products array
        // Frontend will receive this as JSON
        return Ok(products);
    }

    /// <summary>
    /// GET: api/products/1
    /// Returns a single product by ID
    /// Frontend calls this when user clicks on a product for details
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id)
    {
        // Find product by ID and include its category
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        // If product doesn't exist, return 404 Not Found
        if (product == null)
            return NotFound($"Product with ID {id} not found");

        // Return 200 OK with the product data
        return Ok(product);
    }

    /// <summary>
    /// GET: api/products/category/1
    /// Returns all products in a specific category
    /// Frontend calls this when user filters by category
    /// </summary>
    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetProductsByCategory(int categoryId)
    {
        // Get all products that belong to this category
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)  // Filter by category ID
            .OrderBy(p => p.Name)
            .ToListAsync();

        // Even if no products found, return empty array (not 404)
        // This is better UX - shows "No products in this category"
        return Ok(products);
    }

    /// <summary>
    /// GET: api/products/search?query=laptop
    /// Searches products by name or description
    /// Frontend calls this when user uses the search box
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string query)
    {
        // If search query is empty, return all products
        if (string.IsNullOrWhiteSpace(query))
        {
            return await GetAllProducts();
        }

        // Search in product name and description
        // ToLower() makes the search case-insensitive
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Name.ToLower().Contains(query.ToLower()) || 
                       p.Description.ToLower().Contains(query.ToLower()))
            .OrderBy(p => p.Name)
            .ToListAsync();

        return Ok(products);
    }

    /// <summary>
    /// POST: api/products
    /// Creates a new product
    /// Admin functionality - adds new product to catalog
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        // Validate that the category exists
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == product.CategoryId);

        if (!categoryExists)
            return BadRequest($"Category with ID {product.CategoryId} does not exist");

        // Set creation date
        product.CreatedAt = DateTime.UtcNow;

        // Add product to database
        _context.Products.Add(product);
        
        // Save changes to database
        await _context.SaveChangesAsync();

        // Return 201 Created with the new product
        // Also includes location header: /api/products/{id}
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    /// <summary>
    /// PUT: api/products/1
    /// Updates an existing product
    /// Admin functionality - edit product details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
    {
        // Find the existing product
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound($"Product with ID {id} not found");

        // Validate that the new category exists
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == updatedProduct.CategoryId);

        if (!categoryExists)
            return BadRequest($"Category with ID {updatedProduct.CategoryId} does not exist");

        // Update product properties
        product.Name = updatedProduct.Name;
        product.Description = updatedProduct.Description;
        product.Price = updatedProduct.Price;
        product.ImageUrl = updatedProduct.ImageUrl;
        product.Stock = updatedProduct.Stock;
        product.CategoryId = updatedProduct.CategoryId;

        // Save changes to database
        await _context.SaveChangesAsync();

        // Return updated product with category info
        await _context.Entry(product).Reference(p => p.Category).LoadAsync();
        return Ok(product);
    }

    /// <summary>
    /// DELETE: api/products/1
    /// Deletes a product
    /// Admin functionality - remove product from catalog
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        // Find the product
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound($"Product with ID {id} not found");

        // Remove from database
        _context.Products.Remove(product);
        
        // Save changes
        await _context.SaveChangesAsync();

        // Return success message
        return Ok(new { message = $"Product '{product.Name}' deleted successfully" });
    }

    /// <summary>
    /// PATCH: api/products/1/stock
    /// Updates only the stock quantity
    /// Used when products are purchased (decrease stock)
    /// </summary>
    [HttpPatch("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] int newStock)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
            return NotFound($"Product with ID {id} not found");

        // Validate stock is not negative
        if (newStock < 0)
            return BadRequest("Stock cannot be negative");

        // Update stock
        product.Stock = newStock;
        await _context.SaveChangesAsync();

        return Ok(new { productId = id, newStock = product.Stock });
    }
}