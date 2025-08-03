using Microsoft.EntityFrameworkCore;

namespace AspirePostgresRag.Data;


public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TodoDbItem> TodoItems => Set<TodoDbItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoDbItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.IsCompleted).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
