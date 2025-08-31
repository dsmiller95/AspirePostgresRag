﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.ErrorRecoveries;

class ErrorRecoveryDbEntityTypeConfiguration
    : IEntityTypeConfiguration<ErrorRecoveryDb>
{
    public void Configure(EntityTypeBuilder<ErrorRecoveryDb> builder)
    {
        builder.ToTable("ErrorRecoveries");
        
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.NormalizationKey)
            .IsRequired()
            .HasMaxLength(255);
        builder.Property(x => x.ErrorContent)
            .IsRequired()
            .IsFixedLength(false);
        builder.Property(x => x.ErrorResponse)
            .IsRequired()
            .IsFixedLength(false);
    }
}
