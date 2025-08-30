using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.EntityConfigurations;

class TodoDbItemEntityTypeConfiguration
    : IEntityTypeConfiguration<TodoDbItem>
{
    public void Configure(EntityTypeBuilder<TodoDbItem> builder)
    {
        builder.ToTable("TodoItems");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            ;
        
        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(x => x.IsCompleted)
            .IsRequired();
        
        // length of 1536, openai text-embedding-3-small model
        builder.Property(x => x.Embedding)
            .HasColumnType("vector(1536)");
    }
}
