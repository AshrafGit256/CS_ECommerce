using Microsoft.EntityFrameworkCore;
using ECommAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
// This registers the controllers so they can handle API requests
builder.Services.AddControllers();

// This enables API documentation
builder.Services.AddEndpointsApiExplorer();

// This adds Swagger for testing our API
builder.Services.AddSwaggerGen();

// Register the database context
// This connects our application to the SQLite database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS policy
// CORS = Cross-Origin Resource Sharing
// This allows our frontend (HTML/JS) to communicate with our backend API
// Without this, browsers will block the requests for security reasons
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()      // Allow requests from any domain
                   .AllowAnyMethod()      // Allow GET, POST, PUT, DELETE, etc.
                   .AllowAnyHeader();     // Allow any headers in requests
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline
// This section defines how the app handles incoming requests

// Enable Swagger only in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable serving static files (HTML, CSS, JS, images)
// Files in wwwroot folder will be accessible via the browser
app.UseStaticFiles();

// Enable CORS - must be called before MapControllers
app.UseCors("AllowAll");

// Redirect HTTP to HTTPS for security
app.UseHttpsRedirection();

// Enable authorization (we'll use this later for user authentication)
app.UseAuthorization();

// Map controller routes - this makes our API endpoints accessible
app.MapControllers();

// Set the default page to index.html when accessing the root URL
app.MapFallbackToFile("index.html");

// Start the application
app.Run();