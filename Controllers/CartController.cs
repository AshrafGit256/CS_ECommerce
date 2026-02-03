using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommAPI.Data;
using ECommAPI.Models;

namespace ECommAPI.Controllers;

/// <summary>
/// API Controller for managing shopping cart
/// Base route: /api/cart
/// Handles adding, removing, and viewing cart items
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly AppDbContext _context;

    public CartController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// GET: api/cart/{sessionId}
    /// Gets all items in a user's cart
    /// Frontend calls this to display the cart
    /// 
    /// sessionId: A unique identifier for the user's session
    /// In a real app with login, this would be the user's ID
    /// For now, we generate a random ID in the browser (like a cookie)
    /// </summary>
    [HttpGet("{sessionId}")]
    public async Task<IActionResult> GetCart(string sessionId)
    {
        // Get all cart items for this session
        // Include the Product details so we can show name, price, image
        var cartItems = await _context.CartItems
            .Include(ci => ci.Product)           // Load product details
            .ThenInclude(p => p.Category)        // Also load category for each product
            .Where(ci => ci.SessionId == sessionId)  // Filter by session
            .ToListAsync();

        // Calculate the total price
        // For each item: price * quantity, then sum all items
        var total = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);

        // Return cart items and total
        return Ok(new
        {
            items = cartItems,
            totalItems = cartItems.Sum(ci => ci.Quantity),  // Total number of items
            totalPrice = total
        });
    }

    /// <summary>
    /// POST: api/cart
    /// Adds a product to the cart
    /// Frontend calls this when user clicks "Add to Cart"
    /// 
    /// Request body example:
    /// {
    ///   "productId": 1,
    ///   "quantity": 2,
    ///   "sessionId": "abc123"
    /// }
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        // Validate the product exists
        var product = await _context.Products.FindAsync(request.ProductId);

        if (product == null)
            return NotFound($"Product with ID {request.ProductId} not found");

        // Check if there's enough stock
        if (product.Stock < request.Quantity)
            return BadRequest($"Not enough stock. Only {product.Stock} items available");

        // Check if this product is already in the cart for this session
        var existingCartItem = await _context.CartItems
            .FirstOrDefaultAsync(ci => ci.ProductId == request.ProductId &&
                                      ci.SessionId == request.SessionId);

        if (existingCartItem != null)
        {
            // Product already in cart - just increase the quantity
            existingCartItem.Quantity += request.Quantity;

            // Check total quantity doesn't exceed stock
            if (existingCartItem.Quantity > product.Stock)
                return BadRequest($"Not enough stock. Only {product.Stock} items available");
        }
        else
        {
            // Product not in cart - create new cart item
            var cartItem = new CartItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                SessionId = request.SessionId,
                AddedAt = DateTime.UtcNow
            };

            _context.CartItems.Add(cartItem);
        }

        // Save changes to database
        await _context.SaveChangesAsync();

        // Return success message
        return Ok(new { message = "Product added to cart successfully" });
    }

    /// <summary>
    /// PUT: api/cart/{id}
    /// Updates the quantity of a cart item
    /// Frontend calls this when user changes quantity in cart
    /// 
    /// id: The cart item ID (not product ID)
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCartItem(int id, [FromBody] UpdateCartRequest request)
    {
        // Find the cart item
        var cartItem = await _context.CartItems
            .Include(ci => ci.Product)
            .FirstOrDefaultAsync(ci => ci.Id == id);

        if (cartItem == null)
            return NotFound($"Cart item with ID {id} not found");

        // Validate quantity
        if (request.Quantity <= 0)
            return BadRequest("Quantity must be greater than 0");

        // Check stock availability
        if (request.Quantity > cartItem.Product.Stock)
            return BadRequest($"Not enough stock. Only {cartItem.Product.Stock} items available");

        // Update quantity
        cartItem.Quantity = request.Quantity;

        // Save changes
        await _context.SaveChangesAsync();

        return Ok(new { message = "Cart updated successfully" });
    }

    /// <summary>
    /// DELETE: api/cart/{id}
    /// Removes an item from the cart
    /// Frontend calls this when user clicks "Remove" on a cart item
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFromCart(int id)
    {
        // Find the cart item
        var cartItem = await _context.CartItems.FindAsync(id);

        if (cartItem == null)
            return NotFound($"Cart item with ID {id} not found");

        // Remove from database
        _context.CartItems.Remove(cartItem);

        // Save changes
        await _context.SaveChangesAsync();

        return Ok(new { message = "Item removed from cart successfully" });
    }

    /// <summary>
    /// DELETE: api/cart/clear/{sessionId}
    /// Clears all items from a user's cart
    /// Used after checkout or when user wants to empty cart
    /// </summary>
    [HttpDelete("clear/{sessionId}")]
    public async Task<IActionResult> ClearCart(string sessionId)
    {
        // Get all cart items for this session
        var cartItems = await _context.CartItems
            .Where(ci => ci.SessionId == sessionId)
            .ToListAsync();

        // Remove all items
        _context.CartItems.RemoveRange(cartItems);

        // Save changes
        await _context.SaveChangesAsync();

        return Ok(new { message = "Cart cleared successfully" });
    }
}

/// <summary>
/// Request model for adding items to cart
/// This defines what data the frontend must send
/// </summary>
public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string SessionId { get; set; } = string.Empty;
}

/// <summary>
/// Request model for updating cart item quantity
/// </summary>
public class UpdateCartRequest
{
    public int Quantity { get; set; }
}