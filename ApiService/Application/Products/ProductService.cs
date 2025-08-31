using ApiService.Application.Ai;
using Data;
using Data.Products;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace ApiService.Application.Products;

public interface IProductService
{
    Task<List<Product>> GetProducts();
    Task<Product?> GetProduct(int id);
    Task<Product> UpsertProduct(UpsertProduct product);
    Task<bool> DeleteProduct(int id);
    Task<List<RankedProduct>> SearchProducts(string search);
}

public class ProductService(AppDbContext db, IEmbeddingService embeddingService) : IProductService
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

    public async Task<Product> UpsertProduct(UpsertProduct product)
    {
        var existing = await db.Products
            .FirstOrDefaultAsync(x => x.UniqueSku == product.UniqueSku);
        var existingProduct = existing?.ToDomain();
        
        if (existingProduct != null && !product.HasChanges(existingProduct))
        {
            return existingProduct;
        }
        
        var embedding = await embeddingService.GetEmbeddingAsync(product.ScrapedJson);

        if (existingProduct is null)
        {
            var dbItem = ProductDb.From(product, new Vector(embedding));
            var added = db.Products.Add(dbItem);
            await db.SaveChangesAsync();

            return added.Entity.ToDomain();
        }
        else
        {
            var updatedProduct = existingProduct with { Title = product.Title, ScrapedJson = product.ScrapedJson, };
            var updatedItem = ProductDb.From(updatedProduct, new Vector(embedding));
            db.Products.Update(updatedItem);
            await db.SaveChangesAsync();

            return updatedItem.ToDomain();
        }
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

    public async Task<List<RankedProduct>> SearchProducts(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return (await GetProducts())
                .Select(x => RankedProduct.From(x, 0))
                .Take(10)
                .ToList();
        }
        var embedding = await embeddingService.GetEmbeddingAsync(search);
        var vector = new Vector(embedding).EnsureValidTextEmbedding();
        var results = await db.Products
            .Select(x => new { DbItem = x, Distance = x.Embedding.L2Distance(vector) })
            .OrderBy(x => x.Distance)
            .Take(10)
            .Select(x => RankedProduct.From(x.DbItem.ToDomain(), x.Distance))
            .ToListAsync();
        
        return results;
    }
}
