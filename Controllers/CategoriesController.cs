using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommAPI.Data;
using ECommAPI.Models;

namespace ECommAPI.Controllers;

/// <summary>
/// API Controller for managing product categories
/// Base route: /api/categories
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    // Database context - injected automatically by ASP.NET Core
    private readonly AppDbContext _context;

    // Constructor - receives the database context
    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET: api/categories
    /// Returns all categories
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        // Fetch all categories from database
        // Include() loads related products for each category
        var categories = await _context.Categories
            .Include(c => c.Products)  // This loads all products for each category
            .ToListAsync();

        // Return 200 OK with the categories data
        return Ok(categories);
    }

    /// <summary>
    /// GET: api/categories/1
    /// Returns a specific category by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        // Find category by ID and include its products
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        // If not found, return 404 Not Found
        if (category == null)
            return NotFound($"Category with ID {id} not found");

        // Return 200 OK with the category data
        return Ok(category);
    }

    /// <summary>
    /// POST: api/categories
    /// Creates a new category
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] Category category)
    {
        // Add the new category to the database
        _context.Categories.Add(category);

        // Save changes to database
        await _context.SaveChangesAsync();

        // Return 201 Created with the new category
        // Also includes a location header pointing to the new resource
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
    }

    /// <summary>
    /// PUT: api/categories/1
    /// Updates an existing category
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category updatedCategory)
    {
        // Find the existing category
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return NotFound($"Category with ID {id} not found");

        // Update the category properties
        category.Name = updatedCategory.Name;
        category.Description = updatedCategory.Description;

        // Save changes to database
        await _context.SaveChangesAsync();

        // Return 200 OK with updated category
        return Ok(category);
    }

    /// <summary>
    /// DELETE: api/categories/1
    /// Deletes a category
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        // Find the category
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
            return NotFound($"Category with ID {id} not found");

        // Remove from database
        _context.Categories.Remove(category);

        // Save changes
        await _context.SaveChangesAsync();

        // Return 200 OK with success message
        return Ok(new { message = $"Category '{category.Name}' deleted successfully" });
    }
}