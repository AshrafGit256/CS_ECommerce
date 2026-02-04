/* ==========================================
   CONFIGURATION & GLOBAL VARIABLES
   ========================================== */

// API base URL - this is where our backend is running
// Change the port number if your API runs on a different port
const API_URL = 'http://localhost:5151/api';

// Session ID for the shopping cart
// In a real app with login, this would be the user's ID
// For now, we generate a random ID and store it in browser's localStorage
// This allows the cart to persist even if user refreshes the page
let sessionId = localStorage.getItem('sessionId');
if (!sessionId) {
    // Generate a random session ID if one doesn't exist
    sessionId = 'session_' + Math.random().toString(36).substr(2, 9);
    localStorage.setItem('sessionId', sessionId);
}

/* ==========================================
   PAGE INITIALIZATION
   ========================================== */

// This runs when the page loads
// It sets up event listeners and loads initial data
document.addEventListener('DOMContentLoaded', () => {
    console.log('Page loaded. Session ID:', sessionId);
    
    // Load categories and products from the API
    loadCategories();
    loadProducts();
    loadCart();
    
    // Set up event listeners for user interactions
    setupEventListeners();
});

/* ==========================================
   EVENT LISTENERS SETUP
   ========================================== */

function setupEventListeners() {
    // Search functionality
    document.getElementById('searchBtn').addEventListener('click', handleSearch);
    document.getElementById('searchInput').addEventListener('keypress', (e) => {
        if (e.key === 'Enter') handleSearch();
    });
    
    // Cart modal (popup) controls
    document.getElementById('cartIcon').addEventListener('click', openCart);
    document.getElementById('closeCartBtn').addEventListener('click', closeCart);
    document.getElementById('clearCartBtn').addEventListener('click', clearCart);
    document.getElementById('checkoutBtn').addEventListener('click', handleCheckout);
    
    // Close modal when clicking outside of it
    document.getElementById('cartModal').addEventListener('click', (e) => {
        if (e.target.id === 'cartModal') closeCart();
    });
    
    // Logo click - reload all products
    document.querySelector('.logo').addEventListener('click', () => {
        loadProducts();
        document.getElementById('sectionTitle').textContent = 'All Products';
    });
}

/* ==========================================
   API CALLS - CATEGORIES
   ========================================== */

/**
 * Loads all categories from the API
 * API Endpoint: GET /api/categories
 * This is called when the page loads
 */
async function loadCategories() {
    try {
        showLoading(true);
        
        // Make HTTP GET request to the backend API
        // fetch() is a browser function that makes HTTP requests
        const response = await fetch(`${API_URL}/categories`);
        
        // Check if the request was successful
        if (!response.ok) {
            throw new Error('Failed to load categories');
        }
        
        // Convert the response from JSON to JavaScript array
        const categories = await response.json();
        
        console.log('Categories loaded:', categories);
        
        // Display the categories on the page
        displayCategories(categories);
        
    } catch (error) {
        console.error('Error loading categories:', error);
        alert('Failed to load categories. Please try again.');
    } finally {
        showLoading(false);
    }
}

/**
 * Displays categories as clickable buttons
 * When user clicks a category, it filters products by that category
 */
function displayCategories(categories) {
    const container = document.getElementById('categoriesList');
    
    // Clear existing content
    container.innerHTML = '';
    
    // Add "All Products" button
    const allBtn = document.createElement('button');
    allBtn.className = 'category-btn active';
    allBtn.textContent = 'All Products';
    allBtn.addEventListener('click', () => {
        // Remove 'active' class from all buttons
        document.querySelectorAll('.category-btn').forEach(btn => {
            btn.classList.remove('active');
        });
        allBtn.classList.add('active');
        
        // Load all products
        loadProducts();
        document.getElementById('sectionTitle').textContent = 'All Products';
    });
    container.appendChild(allBtn);
    
    // Create a button for each category
    categories.forEach(category => {
        const button = document.createElement('button');
        button.className = 'category-btn';
        button.textContent = category.name;
        
        // When clicked, filter products by this category
        button.addEventListener('click', () => {
            // Update active state
            document.querySelectorAll('.category-btn').forEach(btn => {
                btn.classList.remove('active');
            });
            button.classList.add('active');
            
            // Load products for this category
            loadProductsByCategory(category.id);
            document.getElementById('sectionTitle').textContent = category.name;
        });
        
        container.appendChild(button);
    });
}

/* ==========================================
   API CALLS - PRODUCTS
   ========================================== */

/**
 * Loads all products from the API
 * API Endpoint: GET /api/products
 */
async function loadProducts() {
    try {
        showLoading(true);
        
        // Make API call to get all products
        const response = await fetch(`${API_URL}/products`);
        
        if (!response.ok) {
            throw new Error('Failed to load products');
        }
        
        // Parse JSON response
        const products = await response.json();
        
        console.log('Products loaded:', products);
        
        // Display products on the page
        displayProducts(products);
        
    } catch (error) {
        console.error('Error loading products:', error);
        alert('Failed to load products. Please try again.');
    } finally {
        showLoading(false);
    }
}

/**
 * Loads products filtered by category
 * API Endpoint: GET /api/products/category/{categoryId}
 */
async function loadProductsByCategory(categoryId) {
    try {
        showLoading(true);
        
        // Make API call with category ID in the URL
        const response = await fetch(`${API_URL}/products/category/${categoryId}`);
        
        if (!response.ok) {
            throw new Error('Failed to load products');
        }
        
        const products = await response.json();
        
        console.log(`Products in category ${categoryId}:`, products);
        
        displayProducts(products);
        
    } catch (error) {
        console.error('Error loading products by category:', error);
        alert('Failed to load products. Please try again.');
    } finally {
        showLoading(false);
    }
}

/**
 * Searches products by keyword
 * API Endpoint: GET /api/products/search?query={searchTerm}
 */
async function handleSearch() {
    const searchTerm = document.getElementById('searchInput').value.trim();
    
    if (!searchTerm) {
        loadProducts();
        return;
    }
    
    try {
        showLoading(true);
        
        // Make API call with search query as URL parameter
        const response = await fetch(`${API_URL}/products/search?query=${encodeURIComponent(searchTerm)}`);
        
        if (!response.ok) {
            throw new Error('Search failed');
        }
        
        const products = await response.json();
        
        console.log(`Search results for "${searchTerm}":`, products);
        
        displayProducts(products);
        document.getElementById('sectionTitle').textContent = `Search results for "${searchTerm}"`;
        
    } catch (error) {
        console.error('Error searching products:', error);
        alert('Search failed. Please try again.');
    } finally {
        showLoading(false);
    }
}

/**
 * Displays products as cards in a grid
 */
function displayProducts(products) {
    const container = document.getElementById('productsGrid');
    
    // Clear existing products
    container.innerHTML = '';
    
    // If no products found
    if (products.length === 0) {
        container.innerHTML = '<p style="grid-column: 1/-1; text-align: center; color: #95a5a6; padding: 40px;">No products found</p>';
        return;
    }
    
    // Create a card for each product
    products.forEach(product => {
        const card = createProductCard(product);
        container.appendChild(card);
    });
}

/**
 * Creates a product card HTML element
 */
function createProductCard(product) {
    const card = document.createElement('div');
    card.className = 'product-card';
    
    // Check if product is in stock
    const isOutOfStock = product.stock === 0;
    
    card.innerHTML = `
        <img src="${product.imageUrl}" alt="${product.name}" class="product-image">
        <div class="product-category">${product.category.name}</div>
        <div class="product-name">${product.name}</div>
        <div class="product-description">${product.description}</div>
        <div class="product-footer">
            <div>
                <div class="product-price">$${product.price.toFixed(2)}</div>
                <div class="product-stock">${isOutOfStock ? 'Out of Stock' : `${product.stock} in stock`}</div>
            </div>
            <button 
                class="btn-add-cart" 
                onclick="addToCart(${product.id}, '${product.name}')"
                ${isOutOfStock ? 'disabled' : ''}
            >
                ${isOutOfStock ? 'Out of Stock' : 'Add to Cart'}
            </button>
        </div>
    `;
    
    return card;
}

/* ==========================================
   API CALLS - SHOPPING CART
   ========================================== */

/**
 * Adds a product to the shopping cart
 * API Endpoint: POST /api/cart
 * Sends: { productId, quantity, sessionId }
 */
async function addToCart(productId, productName) {
    try {
        showLoading(true);
        
        // Prepare the data to send to the API
        const requestData = {
            productId: productId,
            quantity: 1,  // Add 1 item by default
            sessionId: sessionId
        };
        
        console.log('Adding to cart:', requestData);
        
        // Make POST request to add item to cart
        // POST is used when creating/adding new data
        const response = await fetch(`${API_URL}/cart`, {
            method: 'POST',  // HTTP method
            headers: {
                'Content-Type': 'application/json'  // Tell server we're sending JSON
            },
            body: JSON.stringify(requestData)  // Convert JavaScript object to JSON string
        });
        
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Failed to add to cart');
        }
        
        const result = await response.json();
        console.log('Add to cart response:', result);
        
        // Show success message
        alert(`${productName} added to cart!`);
        
        // Reload cart to update the count
        loadCart();
        
    } catch (error) {
        console.error('Error adding to cart:', error);
        alert(error.message || 'Failed to add to cart. Please try again.');
    } finally {
        showLoading(false);
    }
}

/**
 * Loads the shopping cart items
 * API Endpoint: GET /api/cart/{sessionId}
 */
async function loadCart() {
    try {
        // Make GET request with session ID in URL
        const response = await fetch(`${API_URL}/cart/${sessionId}`);
        
        if (!response.ok) {
            throw new Error('Failed to load cart');
        }
        
        // Response contains: { items: [], totalItems: 0, totalPrice: 0 }
        const cartData = await response.json();
        
        console.log('Cart loaded:', cartData);
        
        // Update cart badge (number on cart icon)
        updateCartBadge(cartData.totalItems);
        
        // Store cart data globally so openCart() can use it
        window.currentCart = cartData;
        
    } catch (error) {
        console.error('Error loading cart:', error);
    }
}

/**
 * Updates the cart badge (number showing item count)
 */
function updateCartBadge(count) {
    const badge = document.getElementById('cartBadge');
    badge.textContent = count;
    
    // Hide badge if cart is empty
    if (count === 0) {
        badge.style.display = 'none';
    } else {
        badge.style.display = 'inline-block';
    }
}

/**
 * Opens the shopping cart modal
 */
function openCart() {
    const modal = document.getElementById('cartModal');
    modal.classList.add('show');
    
    // Display cart items
    displayCartItems(window.currentCart);
}

/**
 * Closes the shopping cart modal
 */
function closeCart() {
    const modal = document.getElementById('cartModal');
    modal.classList.remove('show');
}

/**
 * Displays cart items in the modal
 */
function displayCartItems(cartData) {
    const container = document.getElementById('cartItems');
    
    // If cart is empty
    if (!cartData || cartData.items.length === 0) {
        container.innerHTML = '<div class="empty-cart">Your cart is empty</div>';
        document.getElementById('totalItems').textContent = '0';
        document.getElementById('totalPrice').textContent = '$0.00';
        return;
    }
    
    // Clear container
    container.innerHTML = '';
    
    // Create HTML for each cart item
    cartData.items.forEach(item => {
        const cartItem = createCartItem(item);
        container.appendChild(cartItem);
    });
    
    // Update totals
    document.getElementById('totalItems').textContent = cartData.totalItems;
    document.getElementById('totalPrice').textContent = `$${cartData.totalPrice.toFixed(2)}`;
}

/**
 * Creates a cart item HTML element
 */
function createCartItem(item) {
    const div = document.createElement('div');
    div.className = 'cart-item';
    
    const itemTotal = (item.product.price * item.quantity).toFixed(2);
    
    div.innerHTML = `
        <img src="${item.product.imageUrl}" alt="${item.product.name}" class="cart-item-image">
        <div class="cart-item-details">
            <div class="cart-item-name">${item.product.name}</div>
            <div class="cart-item-price">$${item.product.price.toFixed(2)} × ${item.quantity} = $${itemTotal}</div>
            <div class="cart-item-quantity">
                <button class="quantity-btn" onclick="updateQuantity(${item.id}, ${item.quantity - 1})">-</button>
                <span class="quantity-display">${item.quantity}</span>
                <button class="quantity-btn" onclick="updateQuantity(${item.id}, ${item.quantity + 1})">+</button>
            </div>
        </div>
        <button class="btn-remove" onclick="removeFromCart(${item.id})">Remove</button>
    `;
    
    return div;
}

/**
 * Updates the quantity of a cart item
 * API Endpoint: PUT /api/cart/{id}
 * Sends: { quantity }
 */
async function updateQuantity(cartItemId, newQuantity) {
    // Don't allow quantity less than 1
    if (newQuantity < 1) {
        removeFromCart(cartItemId);
        return;
    }
    
    try {
        showLoading(true);
        
        console.log(`Updating cart item ${cartItemId} to quantity ${newQuantity}`);
        
        // Make PUT request to update quantity
        // PUT is used for updating existing data
        const response = await fetch(`${API_URL}/cart/${cartItemId}`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ quantity: newQuantity })
        });
        
        if (!response.ok) {
            const error = await response.text();
            throw new Error(error || 'Failed to update quantity');
        }
        
        // Reload cart to show updated data
        await loadCart();
        
        // Refresh the modal display
        openCart();
        
    } catch (error) {
        console.error('Error updating quantity:', error);
        alert(error.message || 'Failed to update quantity. Please try again.');
    } finally {
        showLoading(false);
    }
}

/**
 * Removes an item from the cart
 * API Endpoint: DELETE /api/cart/{id}
 */
async function removeFromCart(cartItemId) {
    if (!confirm('Remove this item from cart?')) {
        return;
    }
    
    try {
        showLoading(true);
        
        console.log(`Removing cart item ${cartItemId}`);
        
        // Make DELETE request
        // DELETE is used for removing data
        const response = await fetch(`${API_URL}/cart/${cartItemId}`, {
            method: 'DELETE'
        });
        
        if (!response.ok) {
            throw new Error('Failed to remove item');
        }
        
        // Reload cart
        await loadCart();
        
        // Refresh modal
        openCart();
        
    } catch (error) {
        console.error('Error removing item:', error);
        alert('Failed to remove item. Please try again.');
    } finally {
        showLoading(false);
    }
}

/**
 * Clears all items from the cart
 * API Endpoint: DELETE /api/cart/clear/{sessionId}
 */
async function clearCart() {
    if (!confirm('Clear all items from cart?')) {
        return;
    }
    
    try {
        showLoading(true);
        
        console.log(`Clearing cart for session ${sessionId}`);
        
        // Make DELETE request to clear cart
        const response = await fetch(`${API_URL}/cart/clear/${sessionId}`, {
            method: 'DELETE'
        });
        
        if (!response.ok) {
            throw new Error('Failed to clear cart');
        }
        
        // Reload cart
        await loadCart();
        
        // Close modal
        closeCart();
        
        alert('Cart cleared successfully');
        
    } catch (error) {
        console.error('Error clearing cart:', error);
        alert('Failed to clear cart. Please try again.');
    } finally {
        showLoading(false);
    }
}

/**
 * Handles checkout process
 * In a real app, this would process payment and create an order
 */
function handleCheckout() {
    if (!window.currentCart || window.currentCart.items.length === 0) {
        alert('Your cart is empty');
        return;
    }
    
    // For now, just show a success message
    // In a real app, you would:
    // 1. Create an order in the database
    // 2. Process payment
    // 3. Update product stock
    // 4. Send confirmation email
    alert(`Checkout successful!\nTotal: $${window.currentCart.totalPrice.toFixed(2)}\n\nIn a real app, payment would be processed here.`);
    
    // Clear cart after checkout
    clearCart();
}

/* ==========================================
   UTILITY FUNCTIONS
   ========================================== */

/**
 * Shows or hides the loading spinner
 */
function showLoading(show) {
    const loading = document.getElementById('loading');
    if (show) {
        loading.classList.add('show');
    } else {
        loading.classList.remove('show');
    }
}