using MarkRestaurant.Data;
using Microsoft.EntityFrameworkCore;
using MarkRestaurant.Models;
namespace MarkRestaurant.Data.Repository;

public class ProductRepository
{
    private readonly MarkRestaurantDbContext _context;
    public ProductRepository(
        MarkRestaurantDbContext context
        ) 
    {
        _context = context;
    }
    
    public async Task<List<Product>> GetAllProducts()
    {
        var products = _context.MenuProducts.ToList();
    
        return products;
    }

    public async Task<Product> GetProductByTitleAndCategoryAsync(string title, string category)
    {
        return await _context.MenuProducts
            .FirstOrDefaultAsync(p => p.Title == title && p.Category == category);
    }

    public async Task<Product> GetProductById(Guid id)
    {
        var product = _context.MenuProducts.Where(p => p.Id == id).FirstOrDefault();

        return product;
    }

    public async Task<bool> DeleteProduct(Product product)
    {
        _context.MenuProducts.Remove(product);
        await _context.SaveChangesAsync(); 
        return true;
    }
    public async Task<bool> AddProduct(Product product)
    {
        _context.MenuProducts.Add(product);
        await _context.SaveChangesAsync();
        return true;
    }
}
