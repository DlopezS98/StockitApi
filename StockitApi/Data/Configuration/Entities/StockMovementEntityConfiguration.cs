using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockitApi.Entities;

namespace StockitApi.Data.Configuration.Entities;

public class StockMovementEntityConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovement", t =>
        {
            t.HasCheckConstraint("CHK__STOCKMOVEMENTS__MOVEMENTTYPE",
                $"[MovementType] IN ('{string.Join("', '", StockMovementType.All)}')");
            t.HasCheckConstraint("CHK__STOCKMOVEMENTS__QUANTITY", "[Quantity] <> 0");
        });

        builder.Property(x => x.Reason)
            .HasMaxLength(250);

        builder.Property(x => x.MovementType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.MovementDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(x => x.Product)
            .WithMany(p => p.StockMovements)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany(w => w.StockMovements)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(u => u.StockMovements)
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
