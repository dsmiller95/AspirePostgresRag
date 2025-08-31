using Data;
using Data.Products;
using Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace ApiService.Application.Products;

public interface IProductService
{
    Task<List<Product>> GetProducts();
    Task<Product?> GetProduct(int id);
    Task<Product> CreateProduct(CreateProduct createRequest);
    Task<IEnumerable<int>> GenerateProducts(int count);
    Task<Product> UpdateProduct(UpdateProduct updateRequest);
    Task<bool> DeleteProduct(int id);
}

public class ProductService(AppDbContext db) : IProductService
{
    public async Task<List<Product>> GetProducts()
    {
        return await db.Products.Select(x => x.ToDomain()).ToListAsync();
    }

    public async Task<Product?> GetProduct(int id)
    {
        var item = await db.Products.FindAsync(id);
        return item?.ToDomain();
    }

    public async Task<Product> CreateProduct(CreateProduct Product)
    {
        var dbItem = new ProductDb
        {
            Title = Product.Title,
            Price = Product.Price,
        };
        var added = db.Products.Add(dbItem);
        await db.SaveChangesAsync();
        return added.Entity.ToDomain();
    }

    public async Task<IEnumerable<int>> GenerateProducts(int count)
    {
        var items = ProductBogus.Generate(count);
        db.Products.AddRange(items.Select(ProductDb.From));
        await db.SaveChangesAsync();
        return items.Select(x => x.Id);
    }

    public async Task<Product> UpdateProduct(UpdateProduct request)
    {
        var existingItem = await db.Products.FindAsync(request.Id);
        if (existingItem is null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        var item = existingItem.ToDomain();
        item = item with
        {
            Title = request.Title,
            Price = request.Price,
        };
        
        var dbItem = ProductDb.From(item);
        db.Products.Update(dbItem);
        await db.SaveChangesAsync();
        return dbItem.ToDomain();
    }

    public async Task<bool> DeleteProduct(int id)
    {
        var item = await db.Products.FindAsync(id);
        if (item is null)
        {
            return false;
        }

        db.Products.Remove(item);
        await db.SaveChangesAsync();
        return true;
    }
}
