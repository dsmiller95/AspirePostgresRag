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

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            ;
        
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(x => x.Price)
            .IsRequired();
    }
}
