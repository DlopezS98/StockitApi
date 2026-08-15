using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockitApi.Entities;

namespace StockitApi.Data.Configuration.Entities;

public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Product");

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.HasIndex(x => x.Name);

        builder.Property(x => x.Description)
            .HasMaxLength(200);

        builder.Property(x => x.Barcode)
            .IsRequired()
            .HasMaxLength(64);
        builder.HasIndex(x => x.Barcode).IsUnique();

        builder.Property(x => x.QrCode)
            .IsRequired()
            .HasMaxLength(128);
        builder.HasIndex(x => x.QrCode).IsUnique();

        builder.Property(x => x.Price)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(x => x.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
