﻿using Data.Products;
using Data.TodoItems;
using Microsoft.EntityFrameworkCore;

namespace Data;


public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoDbItem> TodoItems => Set<TodoDbItem>();

    public DbSet<ProductDb> Products => Set<ProductDb>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.ApplyConfiguration(new TodoDbItemEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ProductDbEntityTypeConfiguration());
    }
}
