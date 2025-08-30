using Data.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Data;


public class TodoDbContext(DbContextOptions<TodoDbContext> options) : DbContext(options)
{
    public DbSet<TodoDbItem> TodoItems => Set<TodoDbItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.ApplyConfiguration(new TodoDbItemEntityTypeConfiguration());
    }
}
