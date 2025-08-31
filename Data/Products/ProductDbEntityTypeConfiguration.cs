using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Products;

class ProductDbEntityTypeConfiguration
    : IEntityTypeConfiguration<ProductDb>
{
    public void Configure(EntityTypeBuilder<ProductDb> builder)
    {
        builder.ToTable("Products");
        
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.UniqueSku).IsUnique();

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            ;
        
        builder.Property(x => x.UniqueSku)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(x => x.ScrapedJson)
            .IsFixedLength(false)
            .IsRequired();
        
        // length of 1536, openai text-embedding-3-small model
        builder.Property(x => x.Embedding)
            .HasColumnType($"vector({VectorLengthCompatibilityWorkaround.EmbeddingLength})");
    }
}
