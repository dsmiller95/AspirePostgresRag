using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.ErrorRecoveries;

class ErrorRecoverySchemaDbEntityTypeConfiguration
    : IEntityTypeConfiguration<ErrorRecoverySchemaDb>
{
    public void Configure(EntityTypeBuilder<ErrorRecoverySchemaDb> builder)
    {
        builder.ToTable("ErrorRecoverySchemas");
        
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.NormalizationKey);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.NormalizationKey)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(x => x.JsonSchema)
            .IsRequired()
            .IsFixedLength(false);
    }
}
