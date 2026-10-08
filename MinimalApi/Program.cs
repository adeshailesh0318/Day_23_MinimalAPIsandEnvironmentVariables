using Day23_MinimalApi.Data;
using Day23_MinimalApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =========================
// GET ALL PRODUCTS
// =========================

app.MapGet("/api/products", async (AppDbContext db) =>
{
    var products = await db.Products.ToListAsync();

    return Results.Ok(products);
});

// =========================
// GET PRODUCT BY ID
// =========================

app.MapGet("/api/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product == null)
    {
        return Results.NotFound(new
        {
            message = "Product not found"
        });
    }

    return Results.Ok(product);
});

// =========================
// CREATE PRODUCT
// =========================

app.MapPost("/api/products", async (Product product, AppDbContext db) =>
{
    db.Products.Add(product);

    await db.SaveChangesAsync();

    return Results.Created(
        $"/api/products/{product.Id}",
        product
    );
});

// =========================
// UPDATE PRODUCT
// =========================

app.MapPut("/api/products/{id}", async (int id, Product updatedProduct, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product == null)
    {
        return Results.NotFound(new
        {
            message = "Product not found"
        });
    }

    product.Name = updatedProduct.Name;
    product.Price = updatedProduct.Price;
    product.Category = updatedProduct.Category;

    await db.SaveChangesAsync();

    return Results.Ok(product);
});

// =========================
// DELETE PRODUCT
// =========================

app.MapDelete("/api/products/{id}", async (int id, AppDbContext db) =>
{
    var product = await db.Products.FindAsync(id);

    if (product == null)
    {
        return Results.NotFound(new
        {
            message = "Product not found"
        });
    }

    db.Products.Remove(product);

    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        message = "Product deleted successfully"
    });
});

app.Run();